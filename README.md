# Gestor de Inventario

Aplicación full stack para gestionar los productos de una pequeña empresa: registro de productos, asociación a una categoría y visibilidad de los productos con stock bajo.

- **Backend:** .NET 10, Clean Architecture + CQRS (MediatR), Entity Framework Core y SQL Server.
- **Frontend:** Angular 20, componentes standalone, Signals, zoneless, `OnPush`, Angular Material y lazy loading.

Regla de negocio principal: un producto tiene **stock bajo** cuando `Stock < StockMinimo`.

## Requisitos previos

| Herramienta | Versión |
|---|---|
| .NET SDK | 10 |
| Node.js y npm | 22 o superior |
| SQL Server | 2019 o superior (Express, LocalDB o Docker) |
| Google Chrome | Solo para ejecutar las pruebas del frontend |

## 1. Base de datos

**Opción A (recomendada): automática.** Al iniciar, la API crea la base `GestorInventarioDB` si no existe y carga los datos iniciales (3 categorías y 6 productos de ejemplo). Solo necesitas un SQL Server accesible.

Si no tienes SQL Server instalado, puedes levantarlo con Docker desde la raíz del repositorio:

```bash
docker compose up -d
```

**Opción B: manual.** Ejecuta `database/01_GestorInventarioDB.sql` en SQL Server. El script crea las tablas y los mismos datos iniciales. La API detecta que ya existen y no duplica datos.

### Cadena de conexión

Se configura en `backend/GestorInventario.API/appsettings.json` (clave `ConnectionStrings:Inventario`). Por defecto apunta al SQL Server de `docker-compose.yml` (`localhost,1433`, usuario `sa`). Esa contraseña es solo de desarrollo: cámbiala para cualquier otro uso.

Para usar SQL Server LocalDB (incluido con Visual Studio en Windows):

```
Server=(localdb)\MSSQLLocalDB;Database=GestorInventarioDB;Trusted_Connection=True;TrustServerCertificate=True
```

También puede sobrescribirse sin editar archivos con la variable de entorno `ConnectionStrings__Inventario`.

## 2. Backend

```bash
cd backend
dotnet restore GestorInventario.sln
dotnet run --project GestorInventario.API
```

La API queda en `http://localhost:5000` y Swagger en `http://localhost:5000/swagger`.
Si la base de datos no está disponible, la API arranca igual y registra el error de conexión en la consola.

### Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/productos` | Lista todos los productos |
| GET | `/api/productos/{id}` | Obtiene un producto |
| POST | `/api/productos` | Crea un producto (responde `201 Created`) |
| PUT | `/api/productos/{id}` | Edita un producto |
| DELETE | `/api/productos/{id}` | Elimina un producto (`204 No Content`) |
| GET | `/api/productos/stock-bajo` | Lista los productos con `Stock < StockMinimo` |
| PATCH | `/api/productos/{id}/stock` | Ajusta el stock: `{ "cantidad": 2, "tipo": "ENTRADA" }` (`ENTRADA` o `SALIDA`) |
| GET | `/api/categorias` | Lista las categorías activas (para el selector) |

Los errores responden con `{ "error": "mensaje" }`: `400` para reglas de negocio o datos inválidos, `404` si el recurso no existe y `500` para errores inesperados (sin exponer detalles internos).

### Validaciones de negocio (en el dominio)

- Nombre obligatorio (máximo 150 caracteres).
- Precio mayor que cero.
- Stock y stock mínimo no negativos.
- La categoría debe existir y estar activa.
- Una salida de stock no puede superar el stock disponible.

### Pruebas del backend

```bash
cd backend
dotnet test GestorInventario.sln
```

## 3. Frontend

En otra terminal:

```bash
cd frontend
npm install
npm start
```

Abre `http://localhost:4200`. La URL de la API se configura en `frontend/src/environments/environment.ts` (por defecto `http://localhost:5000/api`).

### Pruebas del frontend

```bash
cd frontend
npm test
```

(Para una sola ejecución sin modo vigilancia: `npm test -- --watch=false --browsers=ChromeHeadless`).

### Qué incluye la interfaz

- Listado en tabla (Angular Material) con indicador visual y etiqueta para el stock bajo.
- Filtro por categoría y por estado de stock (bajo / normal).
- Formulario reactivo tipado para crear y editar (la categoría se elige de una lista).
- Diálogo para registrar entradas y salidas de stock y confirmación antes de eliminar.
- Mensajes de error de la API mostrados al usuario mediante un interceptor global.

## Arquitectura

### Backend (regla de dependencia estricta)

```
API ──────────► Application ──► Domain
Infrastructure ► Application ──► Domain
```

| Capa | Responsabilidad |
|---|---|
| **Domain** | Entidades `Producto` y `Categoria` con sus reglas (validaciones, stock bajo, ajuste de stock). No depende de ninguna otra capa. |
| **Application** | Commands, Queries y Handlers (CQRS con MediatR), DTOs y puertos (`IProductoRepository`, `ICategoriaRepository`). |
| **Infrastructure** | `PersistenceContext` (EF Core), repositorios concretos y `DbSeeder`. |
| **API** | Controllers delgados, middleware de errores, Swagger e inyección de dependencias en `Program.cs`. |

### Frontend

```
src/app/
  core/                      interceptor de errores y servicio de notificaciones
  shared/components/         diálogo de confirmación reutilizable
  features/productos/
    components/              producto-list, producto-form, ajuste-stock-dialog
    services/                servicio HTTP y store con Signals
    models/                  interfaces tipadas
    routes.ts                rutas con lazy loading
  app.config.ts              providers globales (zoneless, router, HttpClient)
  app.routes.ts
```

Cumple el estándar: sin `any`, sin NgModules, sin `::ng-deep` ni `!important`, sin estilos inline, `@if`/`@for`, `OnPush` en todos los componentes, SCSS con BEM y personalización por tokens de Angular Material.

## Respuestas al cuestionario técnico

Ver [CUESTIONARIO_SUSTENTACION.md](CUESTIONARIO_SUSTENTACION.md).

## Alcance

Implementado: todo lo requerido en las secciones 3.3 y 3.4 de la prueba, más pruebas unitarias y un workflow de integración continua (`.github/workflows/ci.yml`).
No implementado por ser opcional: CRUD de categorías, tabla de movimientos de inventario, autenticación, paginación y despliegue en la nube.
