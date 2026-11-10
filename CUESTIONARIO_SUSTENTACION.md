# Cuestionario técnico — respuestas y guía de sustentación

## Backend 1. ¿Por qué el Controller no debe acceder directamente al PersistenceContext?

El controlador pertenece a la capa API y su responsabilidad es recibir solicitudes HTTP, validar el formato de entrada y delegar el caso de uso a Application. Si accede directamente al DbContext, mezcla presentación, lógica de aplicación y persistencia. Eso aumenta el acoplamiento, dificulta las pruebas y hace que las reglas de negocio queden dispersas. Al usar Commands/Queries y Handlers, cada caso de uso queda aislado y la persistencia se consume mediante una interfaz. Se gana mantenibilidad, testabilidad y la posibilidad de cambiar EF Core o SQL Server sin modificar el controlador ni el dominio.

**Cómo decirlo:** “El Controller es la puerta de entrada, no el lugar donde se resuelven los casos de negocio. Envía una intención a Application; el Handler coordina el caso de uso y usa una abstracción de repositorio”.

## Backend 2. ¿Por qué no enviar el correo dentro de CrearProductoCommandHandler?

Porque el Handler terminaría teniendo dos responsabilidades: crear el producto y gestionar una notificación externa. Eso incrementa el acoplamiento y hace más difícil probar y mantener el caso de uso. Una alternativa es publicar un evento de dominio o de aplicación, por ejemplo `ProductoCreado`, y manejarlo con un handler independiente que invoque una interfaz `INotificadorCompras`. También podría utilizarse un patrón Outbox si se requiere confiabilidad ante fallos. El principio relacionado es Responsabilidad Única (SRP); también se favorece inversión de dependencias y bajo acoplamiento.

**Cómo decirlo:** “Crear el producto y notificar compras son responsabilidades distintas. El evento permite que un manejador independiente envíe el correo sin contaminar el caso de uso principal”.

## Frontend 1. ¿Por qué standalone en vez de NgModules? ¿Qué aporta al lazy loading?

Los componentes standalone declaran directamente sus dependencias en `imports` y no necesitan pertenecer a un NgModule. Esto reduce configuración y hace más explícita la composición de la aplicación. En combinación con `loadChildren` o `loadComponent`, el router puede cargar una feature cuando el usuario navega hacia ella, separando el código por funcionalidad y evitando descargar todo desde el inicio.

**Cómo decirlo:** “Standalone elimina el módulo contenedor y permite que cada feature declare lo que necesita. El router carga la feature bajo demanda, lo que mejora la organización y puede reducir la carga inicial”.

## Frontend 2. ¿Por qué OnPush y qué implica para actualizar datos?

OnPush evita revisar innecesariamente un componente en cada ciclo de detección tradicional. Angular comprueba el componente cuando cambian sus entradas, ocurre un evento en su vista o se actualizan mecanismos reactivos reconocidos, como Signals. Por eso conviene actualizar el estado de forma inmutable o mediante Signals, en vez de mutar objetos o arreglos sin notificar el cambio.

**Cómo decirlo:** “OnPush busca rendimiento: el componente se actualiza cuando hay un cambio relevante. Signals hacen explícita la actualización del estado y notifican a la vista”.

## CI/CD, Docker y Kubernetes 1. ¿Por qué YAML versionado y no kubectl create manual?

Los manifiestos YAML guardados en Git permiten revisar cambios, conocer el historial, reproducir ambientes y mantener la infraestructura como código. Los comandos manuales son difíciles de auditar y repetir y pueden generar diferencias entre ambientes. Con YAML, el estado deseado puede aplicarse de forma consistente mediante `kubectl apply`.

**Cómo decirlo:** “El YAML convierte la configuración de infraestructura en código revisable y reproducible; no depende de recordar comandos manuales”.

## CI/CD, Docker y Kubernetes 2. ¿Por qué una imagen Docker?

La imagen empaqueta la aplicación y sus dependencias en una unidad reproducible. Reduce el clásico problema de “funciona en mi máquina” porque el entorno de ejecución queda definido. Facilita mover la aplicación entre desarrollo, pruebas y producción, escalar instancias y desplegar versiones consistentes.

**Cómo decirlo:** “Docker empaqueta la aplicación con lo que necesita para ejecutarse. Así reducimos diferencias entre máquinas y hacemos el despliegue más repetible”.

## CI/CD, Docker y Kubernetes 3. ¿Por qué separar Build y Release/Deploy?

Build compila, ejecuta validaciones y produce un artefacto versionado. Release/Deploy toma ese artefacto y lo publica en un ambiente. Separarlos permite promover exactamente el mismo artefacto entre ambientes sin recompilarlo, aplicar aprobaciones y permisos distintos, y evitar que un cambio de despliegue altere el resultado de compilación. Mitiga el riesgo de desplegar código no validado o diferente al que pasó las pruebas.

**Cómo decirlo:** “Build demuestra que el código se puede construir; Deploy publica el artefacto ya generado. Separarlos permite validar, aprobar y promover una versión controlada”.

---

# Guion breve para la sustentación

1. **Presentación:** “Desarrollé un Gestor de Inventario para administrar productos y controlar existencias”.
2. **Base de datos:** “Hay una relación uno a muchos: una categoría puede tener varios productos y cada producto pertenece a una categoría”.
3. **Backend:** “Organicé el backend con Clean Architecture. Domain contiene las entidades; Application contiene los casos de uso y CQRS; Infrastructure implementa el acceso a SQL Server; API expone los endpoints”.
4. **CQRS:** “Separé consultas (Queries) de operaciones que modifican datos (Commands). Cada una tiene un Handler”.
5. **Frontend:** “Angular está organizado por features. La pantalla usa componentes standalone, OnPush, Signals, formularios reactivos tipados y carga diferida”.
6. **Regla de stock bajo:** “Un producto tiene stock bajo cuando `Stock < StockMinimo`; esa condición se muestra en la tabla y existe un endpoint específico”.
7. **Demostración:** crear un producto, editarlo, filtrar por categoría, ajustar entrada/salida, revisar stock bajo y eliminarlo.
8. **Cierre:** “La solución busca separar responsabilidades, facilitar pruebas y permitir que las capas evolucionen con bajo acoplamiento”.

# Preguntas que podrían hacerte
- **¿Qué es CQRS?** Separar las operaciones de lectura de las de escritura.
- **¿Qué es un Handler?** La clase que ejecuta un caso de uso representado por un Command o Query.
- **¿Qué es un repositorio?** Una abstracción para consultar y persistir entidades sin acoplar Application al ORM.
- **¿Qué significa stock bajo?** Que el stock actual es menor al stock mínimo configurado.
- **¿Por qué validar en backend y frontend?** El frontend mejora la experiencia, pero el backend protege las reglas aunque la API se invoque sin interfaz.
- **¿Qué es un Signal?** Una primitiva reactiva de Angular para mantener y propagar estado.
- **¿Qué es OnPush?** Una estrategia de detección de cambios que reduce comprobaciones innecesarias.

# Decisiones de este proyecto que debes saber justificar
- **¿Por qué las validaciones están en `Producto` y no en el controller ni en el handler?** Son reglas del negocio (precio > 0, stock >= 0, nombre obligatorio). En el dominio se aplican siempre, sin importar quién use la entidad, y se prueban sin base de datos ni HTTP.
- **¿Qué pasa cuando se incumple una regla?** El dominio lanza `DominioException`; el middleware la traduce a `400` con `{ "error": "mensaje" }`. Si el producto no existe, `NoEncontradoException` se traduce a `404`. Los errores inesperados devuelven `500` sin detalles internos y quedan en el log.
- **¿Por qué `Stock < StockMinimo` se repite en el dominio y en el repositorio?** La propiedad `TieneStockBajo` expresa la regla en el dominio; en la consulta del repositorio debe escribirse como expresión LINQ para que EF Core la traduzca a SQL (EF no traduce propiedades calculadas).
- **¿Por qué el repositorio no devuelve entidades a la API?** Application mapea a DTOs; la API nunca expone entidades de dominio.
- **¿Para qué sirve `DbSeeder`?** Crea el esquema si no existe y carga datos iniciales solo si las tablas están vacías, así que se puede ejecutar varias veces sin duplicar datos.
- **¿Por qué un store con Signals y no estado en el componente?** Lista, formulario y diálogos comparten los mismos datos. El store tiene una única fuente de verdad, y sus actualizaciones reemplazan los arreglos (inmutabilidad) para que los componentes `OnPush` se repinten.
- **¿Para qué el interceptor de errores?** Centraliza el manejo de errores HTTP: muestra el mensaje que envía la API sin repetir `catchError` en cada componente.
- **¿Por qué `withComponentInputBinding`?** Permite recibir el parámetro `:id` de la ruta como un `input()` del componente, sin inyectar `ActivatedRoute`.
- **¿Cómo corre el proyecto sin crear la base a mano?** `EnsureCreated` genera las tablas desde el modelo de EF Core al iniciar la API; el script SQL es una alternativa equivalente.
