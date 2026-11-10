-- Script de base de datos (equivalente al esquema que genera la API al iniciar).
-- Úsalo solo si prefieres crear la base manualmente; si no, basta con iniciar la API.
CREATE DATABASE GestorInventarioDB;
GO
USE GestorInventarioDB;
GO

CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT (1),
    CONSTRAINT PK_Categorias PRIMARY KEY (Id)
);
GO

CREATE TABLE Productos (
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT (0),
    StockMinimo INT NOT NULL DEFAULT (5),
    CategoriaId INT NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias (Id),
    CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0),
    CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
    CONSTRAINT CK_Productos_StockMinimo CHECK (StockMinimo >= 0)
);
GO

-- Datos iniciales (seed)
INSERT INTO Categorias (Nombre) VALUES (N'Electrónica'), (N'Oficina'), (N'Aseo');
GO

INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, StockMinimo, CategoriaId)
VALUES
(N'Mouse inalámbrico', N'Mouse óptico USB', 45000, 12, 5, 1),
(N'Teclado mecánico', N'Teclado para oficina', 160000, 2, 5, 1),
(N'Cuaderno', N'Cuaderno de 100 hojas', 8500, 30, 10, 2),
(N'Resma de papel carta', N'500 hojas', 22000, 3, 10, 2),
(N'Detergente', N'Detergente multiusos', 12000, 3, 8, 3),
(N'Jabón líquido', N'Jabón líquido 1 L', 9500, 25, 8, 3);
GO
