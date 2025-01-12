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


--MODELO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Modelo')
BEGIN
    CREATE TABLE Modelo (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL UNIQUE,
		Vram TINYINT NOT NULL,
		Precio DECIMAL(8,2) NOT NULL
    );
END;
GO


--GRAFICA
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Graphic')
BEGIN
    CREATE TABLE Graphic (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MarcaId INT,
		ModeloId INT,
		SerieId INT,
		Estado BIT NOT NULL DEFAULT 0,

        FOREIGN KEY (MarcaId) REFERENCES Marca(Id),
		FOREIGN KEY (SerieId) REFERENCES Serie(Id),
		FOREIGN KEY (ModeloId) REFERENCES Modelo(Id)
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

--STOCK
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stock')
BEGIN
    CREATE TABLE Stock (
        Id INT PRIMARY KEY IDENTITY(1,1),
        GraficaId INT UNIQUE NOT NULL,
        Cantidad INT DEFAULT 0,
        FechaUltimaModificacion VARCHAR(20) NOT NULL,

        FOREIGN KEY (GraficaId) REFERENCES Graphic(Id)
    );
END;
GO

-- MOVIMIENTO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Movimiento')
BEGIN
	CREATE TABLE Movimiento (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		TipoMovimientoId INT,
		Fecha VARCHAR(20),
		Cantidad INT,
		StockId INT, 

		FOREIGN KEY (TipoMovimientoId) REFERENCES TipoMovimiento(Id),
		FOREIGN KEY (StockId) REFERENCES Stock(Id) 
	);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Unidades')
BEGIN
CREATE TABLE Unidades (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StockId INT,              
    Licencia UNIQUEIDENTIFIER DEFAULT NEWID(),
    FechaAdicion VARCHAR(20),  

    FOREIGN KEY (StockId) REFERENCES Stock(Id) 
);
END
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
CREATE NONCLUSTERED INDEX Modelo ON Graphic (ModeloId);
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
            m.Nombre AS Marca, 
            s.Nombre AS Serie, 
			n.Nombre as Modelo, 
            n.VRAM, 
            n.Precio,
            g.Estado,
            a.Id AS AdjuntoId,
            a.Ruta
        FROM 
            Graphic g
        INNER JOIN 
            Marca m ON g.MarcaID = m.Id
		INNER JOIN 
            Serie s ON g.SerieId = s.Id
		INNER JOIN 
            Modelo n ON g.ModeloId = n.Id
        LEFT JOIN 
            Adjunto a ON g.Id = a.GraphicID
    )
    SELECT 
        GraphicId,       
        Marca,
        Serie,
		Modelo,
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

CREATE OR ALTER PROCEDURE InsertModel
    @type CHAR(1),            -- Marca del Modelo
    @nombre NVARCHAR(50),     
    @vram TINYINT, 
    @precio DECIMAL(8,2)
AS
BEGIN
    -- Declaración de la variable fullname
    DECLARE @fullname NVARCHAR(50);
    
    -- Evaluación del tipo (type) con un CASE
    SET @fullname = CASE
                        WHEN @type = 'G' THEN 'RTX ' + @nombre
                        WHEN @type = 'R' THEN 'RX ' + @nombre
                        ELSE @nombre -- En caso de que no sea 'G' ni 'R', se usa el nombre tal cual
                    END;
    
    -- Inserción en la tabla Modelo
    INSERT INTO Modelo (nombre, vram, precio)
    VALUES (@fullname, @vram, @precio);
END;
GO


CREATE OR ALTER PROCEDURE InsertarGrafica
    @MarcaID INT,
    @ModeloId INT,
	@SerieId INT
AS
BEGIN
    DECLARE @GraphicId INT;
	DECLARE @Model NVARCHAR(50);

    INSERT INTO Graphic (MarcaId, SerieId, ModeloId, Estado)
    VALUES (@MarcaID, @SerieId, @ModeloId, DEFAULT);

    -- Obtener el ID de la gráfica insertada
    SET @GraphicId = SCOPE_IDENTITY();

    -- Devolver el ID de la gráfica insertada
    SELECT @GraphicId;
END;
GO

CREATE OR ALTER PROCEDURE ActualizarGrafica
    @GraphicId INT,
    @MarcaId INT,
    @ModeloId INT,
	@SerieId INT

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
            ModeloId = @ModeloId,
			SerieId = @SerieId
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

    -- Obtener el nombre del modelo relacionado con el GraphicId
    SELECT @Modelo = m.Nombre
    FROM Graphic g
    INNER JOIN Modelo m ON g.ModeloId = m.Id
    WHERE g.Id = @GraphicId;

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

--INSERCCION DE UNIDAD
CREATE OR ALTER PROCEDURE InsertarUnidad
    @StockId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaActual VARCHAR(20);

    -- Obtener la fecha actual formateada usando la función personalizada
    SET @FechaActual = dbo.DateFormatter(GETDATE());

    -- Insertar la unidad en la tabla Unidades, generando automáticamente el valor para Licencia
    INSERT INTO Unidades (StockId, Licencia, FechaAdicion)
    VALUES (@StockId, DEFAULT, @FechaActual);
END;
GO


--DETONADORES

CREATE OR ALTER TRIGGER StartManagement
ON Graphic
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarar variables para los datos de la gráfica insertada
    DECLARE @GraphicId INT, @FechaActual VARCHAR(20)

    -- Obtener el ID de la gráfica insertada
    SELECT @GraphicId = Id FROM INSERTED;

    -- Formatear la fecha actual usando la función personalizada
    SET @FechaActual = dbo.DateFormatter(GETDATE());

    -- Crear un nuevo registro de stock con el valor predeterminado
    INSERT INTO Stock (GraficaId, Cantidad, FechaUltimaModificacion)
    VALUES (@GraphicId, DEFAULT, @FechaActual);

END;
GO

CREATE OR ALTER TRIGGER MakeManagement
ON Unidades
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarar variables para datos auxiliares
    DECLARE @FechaActual VARCHAR(20), @TipoMovimientoId INT;

    -- Obtener la fecha actual formateada usando la función personalizada
    SET @FechaActual = dbo.DateFormatter(GETDATE());

    -- Obtener el ID del tipo de movimiento "Ingreso"
    SELECT @TipoMovimientoId = Id FROM TipoMovimiento WHERE Nombre = 'Ingreso';

    -- Actualizar la cantidad en la tabla Stock consultado el numero de registros referidos al StockId
	UPDATE Stock
	SET Cantidad = (SELECT COUNT(*) FROM Unidades WHERE StockId = Stock.Id),
		FechaUltimaModificacion = @FechaActual
	--Consultamos los Id de stock usados en los registros hechos a Unidades
	--(La tabla virtual INSERTED tambien los tiene por lo que seria mas sencillo consultar a esta ultima)
	WHERE Id IN (SELECT StockId FROM INSERTED);

    -- Registrar el movimiento en la tabla Movimiento después de actualizar el stock
    INSERT INTO Movimiento (TipoMovimientoId, Fecha, Cantidad, StockId)
    SELECT @TipoMovimientoId, @FechaActual, COUNT(*), StockId
    FROM INSERTED
    GROUP BY StockId;
END;
GO



-- INSERCCIONES
INSERT INTO Marca (Nombre) VALUES ('Nvidia'), ('AMD');
INSERT INTO TipoMovimiento (Nombre) VALUES ('Ingreso'), ('Devolución'), ('Venta');
INSERT INTO Serie (Nombre) VALUES ('2000'), ('3000'), ('4000'), ('5000'), ('6000'), ('7000');
INSERT INTO TipoAdjunto (Nombre) VALUES ('Cover'), ('Manual');

EXEC InsertModel @type = 'G', @nombre = '2060', @vram = 6, @precio = 219.99;
EXEC InsertModel @type = 'G', @nombre = '2060 Super', @vram = 8, @precio = 239.99;
EXEC InsertModel @type = 'G', @nombre = '2070', @vram = 8, @precio = 299.99;
EXEC InsertModel @type = 'G', @nombre = '2070 Super', @vram = 8, @precio = 349.99;
EXEC InsertModel @type = 'R', @nombre = '6600', @vram = 8, @precio = 279.99;
EXEC InsertModel @type = 'R', @nombre = '6600 XT', @vram = 8, @precio = 349.99;
EXEC InsertModel @type = 'R', @nombre = '6700', @vram = 10, @precio = 449.99;
EXEC InsertModel @type = 'R', @nombre = '6700 XT', @vram = 12, @precio = 479.99;
EXEC InsertModel @type = 'G', @nombre = '2080', @vram = 8, @precio = 499.99;
EXEC InsertModel @type = 'R', @nombre = '6800', @vram = 16, @precio = 599.99;
EXEC InsertModel @type = 'G', @nombre = '2080 Super', @vram = 8, @precio = 599.99;
EXEC InsertModel @type = 'G', @nombre = '2080 TI', @vram = 11, @precio = 799.99;
GO

--PRUEBAS
SELECT * FROM Modelo
GO

SELECT * FROM Adjunto
GO

SELECT * FROM Graphic
GO

EXEC ListarGraficas 1, 3
GO

SELECT * FROM Movimiento
GO

SELECT * FROM Stock
GO