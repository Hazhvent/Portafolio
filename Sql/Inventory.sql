-- CREACION DE LA BASE DE DATOS
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'INVENTORY')
    CREATE DATABASE [INVENTORY]            
GO

-- MONTAR BASE DE DATOS
USE INVENTORY
GO

-- TABLAS

--MARCA
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Marca')
BEGIN
    CREATE TABLE Marca (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL UNIQUE
    );
END;
GO

--SERIE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Serie')
BEGIN
    CREATE TABLE Serie (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL UNIQUE
    );
END;
GO


--GRAFICA
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Graphic')
BEGIN
    CREATE TABLE Graphic (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MarcaId INT,
        SerieId INT,
        Modelo NVARCHAR(100) NOT NULL UNIQUE, --NOMBRE DE LA TARJETA
        Vram TINYINT NOT NULL,
        Precio DECIMAL(8,2) NOT NULL,
		Estado BIT NOT NULL,
        FOREIGN KEY (MarcaId) REFERENCES Marca(Id),
        FOREIGN KEY (SerieId) REFERENCES Serie(Id)
    );
END;
GO

--TIPO DE MOVIMIENTO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TipoMovimiento')
BEGIN
    CREATE TABLE TipoMovimiento (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre VARCHAR(50) NOT NULL -- (Ingreso, Devolución, etc)
    );
END;
GO

-- MOVIMIENTO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Movimiento')
BEGIN
    CREATE TABLE Movimiento (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TipoMovimientoId INT NOT NULL, 
        Fecha VARCHAR(20) NOT NULL,
        Cantidad INT NOT NULL,
        GraphicId INT NOT NULL, 

		FOREIGN KEY (GraphicId) REFERENCES Graphic(Id),
        FOREIGN KEY (TipoMovimientoId) REFERENCES TipoMovimiento(Id)
    );
END;
GO

--STOCK
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stock')
BEGIN
    CREATE TABLE Stock (
        Id INT PRIMARY KEY IDENTITY(1,1),
        GraficaId INT UNIQUE NOT NULL,
        Cantidad INT DEFAULT 0,
        FechaUltimaModificacion VARCHAR(20) NOT NULL,

        FOREIGN KEY (GraficaId) REFERENCES Graphic(Id),

    );
END;
GO


--TIPO DE ADJUNTO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TipoAdjunto')
BEGIN
    CREATE TABLE TipoAdjunto (
        Id TINYINT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL
    );
END
GO

--ADJUNTO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Adjunto')
BEGIN
    CREATE TABLE Adjunto (
        Id INT PRIMARY KEY IDENTITY(1,1),
		GraphicId INT NOT NULL,
		TipoId TINYINT NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Ruta NVARCHAR(255) NOT NULL,
		Peso BIGINT NOT NULL,
        FOREIGN KEY (GraphicId) REFERENCES Graphic(Id) ON DELETE CASCADE,
		FOREIGN KEY (TipoId) REFERENCES TipoAdjunto(Id) ON DELETE CASCADE
    );
END;


--INDICES NO AGRUPADOS
CREATE NONCLUSTERED INDEX Marca ON Graphic (MarcaId);
CREATE NONCLUSTERED INDEX Serie ON Graphic (SerieId);

-- INSERCCIONES
INSERT INTO Marca (Nombre) VALUES ('Nvidia'), ('AMD');
INSERT INTO TipoMovimiento (Nombre) VALUES ('Ingreso'), ('Devolución'), ('Venta');
INSERT INTO Serie (Nombre) VALUES ('2000'), ('3000'), ('4000'), ('5000'), ('6000'), ('7000');
INSERT INTO TipoAdjunto (Nombre) VALUES ('Cover'), ('Manual');
GO

--FUNCIONES
CREATE OR ALTER FUNCTION KilobytesConverter
(
    @Bytes BIGINT
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    -- Convertir bytes a kilobytes dividiendo por 1024
    RETURN CAST(@Bytes AS DECIMAL(18, 2)) / 1024;
END;
GO

CREATE OR ALTER FUNCTION DateFormatter
(
    @Fecha DATETIME
)
RETURNS VARCHAR(20)
AS
BEGIN
-- Asigna el formato "dd/MM/yy HH:mm:ss"
    RETURN FORMAT(@Fecha, 'dd/MM/yy HH:mm:ss');
END;
GO


-- PROCEDIMIENTOS ALMACENADOS
CREATE OR ALTER PROCEDURE ListarGraficas
    @page INT,            -- Número de página solicitada
    @itemsPerPage INT     -- Cantidad de elementos por página
AS
BEGIN
    SET NOCOUNT ON;

    -- Calcular el índice de inicio para la paginación
    DECLARE @startRow INT = (@page - 1) * @itemsPerPage;

    -- Seleccionar las gráficas y sus adjuntos
    WITH GraficasConAdjuntos AS (
        SELECT 
            g.Id AS GraphicId,
            g.Modelo, 
            m.Nombre AS Marca, 
            s.Nombre AS Serie, 
            g.VRAM, 
            g.Precio,
            g.Estado,
            a.Id AS AdjuntoId,
            a.Ruta
        FROM 
            Graphic g
        INNER JOIN 
            Marca m ON g.MarcaID = m.Id
        INNER JOIN 
            Serie s ON g.SerieID = s.Id
        LEFT JOIN 
            Adjunto a ON g.Id = a.GraphicID
    )
    SELECT 
        GraphicId,
        Modelo,
        Marca,
        Serie,
        VRAM,
        Precio,
        Estado,
        STRING_AGG(CONCAT(AdjuntoId, ':', Ruta), ',') AS Adjuntos
    FROM 
        GraficasConAdjuntos
    GROUP BY 
        GraphicId, Modelo, Marca, Serie, VRAM, Precio, Estado
    ORDER BY 
        GraphicId
    OFFSET 
        @startRow ROWS FETCH NEXT @itemsPerPage ROWS ONLY;

    -- Seleccionar el total de gráficas
    SELECT COUNT(*) AS TotalRecords FROM Graphic;
END;
GO


CREATE OR ALTER PROCEDURE InsertarGrafica
    @MarcaID INT,
    @SerieId INT,
    @Modelo NVARCHAR(100),
    @Vram TINYINT,
    @Precio DECIMAL(18,2)
AS
BEGIN
    DECLARE @GraphicId INT;

    INSERT INTO Graphic (MarcaId, SerieId, Modelo, Vram, Precio, Estado)
    VALUES (@MarcaID, @SerieId, @Modelo, @Vram, @Precio, 1);

    -- Obtener el ID de la gráfica insertada
    SET @GraphicId = SCOPE_IDENTITY();

    -- Devolver el ID de la gráfica insertada
    SELECT @GraphicId;
END;
GO


CREATE OR ALTER PROCEDURE ActualizarGrafica
    @GraphicId INT,
    @MarcaId INT,
    @SerieId INT,
    @Modelo NVARCHAR(100),
    @Vram TINYINT,
    @Precio DECIMAL(18,2)
AS
BEGIN
    DECLARE @Response BIT;

    -- Comprobar si la gráfica existe
    IF EXISTS (SELECT 1 FROM Graphic WHERE Id = @GraphicId)
    BEGIN
        -- Actualizar los datos de la gráfica
        UPDATE Graphic
        SET 
            MarcaId = @MarcaId,
            SerieId = @SerieId,
            Modelo = @Modelo,
            Vram = @Vram,
            Precio = @Precio
        WHERE Id = @GraphicId;

        -- Indicar que la actualización fue exitosa
        SET @Response = 1; -- true
    END
    ELSE
    BEGIN
        -- Indicar que la gráfica no fue encontrada
        SET @Response = 0; -- false
    END

    -- Devolver el resultado de la operación
    SELECT @Response AS Resultado;
END;
GO


CREATE OR ALTER PROCEDURE GetModel
    @GraphicId INT
AS
BEGIN
    DECLARE @Modelo NVARCHAR(100);

    SELECT @Modelo = Modelo
    FROM Graphic
    WHERE Id = @GraphicId;

    -- Devolver el modelo encontrado
    SELECT @Modelo AS Modelo;
END;
GO

CREATE OR ALTER PROCEDURE MakeAttached
    @Graphic INT,
    @Tipo TINYINT,
    @Nombre NVARCHAR(255),
    @Ruta NVARCHAR(255),
    @Peso BIGINT
AS
BEGIN
    DECLARE @Response BIT = 0;
    DECLARE @PesoKB DECIMAL(18, 2);

    -- Convertir el peso a kilobytes usando la función
    SET @PesoKB = dbo.KilobytesConverter(@Peso);

    IF EXISTS (SELECT 1 FROM Graphic WHERE Id = @Graphic)
    BEGIN
        IF EXISTS (SELECT 1 FROM Adjunto WHERE GraphicId = @Graphic AND TipoId = @Tipo)
        BEGIN
            UPDATE Adjunto 
            SET Peso = @PesoKB
            WHERE GraphicId = @Graphic AND TipoId = @Tipo;

            SET @Response = 1;  
        END
        ELSE
        BEGIN
            INSERT INTO Adjunto (GraphicId, TipoId, Nombre, Ruta, Peso)
            VALUES (@Graphic, @Tipo, @Nombre, @Ruta, @PesoKB);

            SET @Response = 1; 
        END
    END    

    SELECT @Response AS Response;
END;
GO

--SWITCH GRAFICA (INCLUYE "SOFT DELETE")
CREATE OR ALTER PROCEDURE CambiarEstado
    @GraphicId INT
AS
BEGIN
    DECLARE @Response BIT;

    -- Comprobar si la gráfica existe
    IF EXISTS (SELECT 1 FROM Graphic WHERE Id = @GraphicId)
    BEGIN
        -- Invertir el valor del estado de la gráfica
        UPDATE Graphic
        SET Estado = ~Estado  -- Cambia el valor al opuesto (1 a 0, 0 a 1)
        WHERE Id = @GraphicId;

        -- Indicar que la actualización fue exitosa
        SET @Response = 1; -- true
    END
    ELSE
    BEGIN
        -- Indicar que la gráfica no fue encontrada
        SET @Response = 0; -- false
    END

    -- Devolver el resultado de la operación
    SELECT @Response AS Resultado;
END;
GO


--PRUEBAS
SELECT * FROM Adjunto
GO

SELECT * FROM Graphic
GO

EXEC ListarGraficas 1, 3
GO