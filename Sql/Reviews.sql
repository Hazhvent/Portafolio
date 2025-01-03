--CREACION DE LA BASE DE DATOS
IF NOT EXISTS (SELECT *FROM sys.databases WHERE name = 'REVIEWS')
        CREATE DATABASE [REVIEWS]            
GO

--MONTAR BASE DE DATOS
USE REVIEWS
GO

--TABLAS

--CLIENTE
IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='CLIENTE')
CREATE TABLE CLIENTE (
ID		    INT IDENTITY (1,1) PRIMARY KEY,
DNI			CHAR(8) NOT NULL,
NOMBRES		VARCHAR(50) NOT NULL
)
GO

--RESEÑAS
IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='RESEÑA')
CREATE TABLE RESEÑA (
ID   			INT IDENTITY (1,1) PRIMARY KEY,
CLIENTE			INT NOT NULL,
RESEÑA			VARCHAR(320) NOT NULL,
PUNTUACION	    TINYINT NOT NULL,

FOREIGN KEY (CLIENTE) REFERENCES CLIENTE
)
GO

--PROCEDIMIENTOS ALMACENADOS

--HACER RESEÑA
CREATE OR ALTER PROCEDURE MAKE_REVIEW @DNI CHAR(8), @RESEÑA VARCHAR(320), @PUNTOS TINYINT
AS
BEGIN
DECLARE @RESPONSE INT, @CLIENTE AS INT
	IF NOT EXISTS (SELECT ID FROM CLIENTE WHERE @DNI = CLIENTE.DNI)
		BEGIN
			SET @RESPONSE = 0
		END
	ELSE
		BEGIN
		SELECT @CLIENTE = ID FROM CLIENTE WHERE @DNI = CLIENTE.DNI
		IF NOT EXISTS (SELECT ID FROM RESEÑA WHERE @CLIENTE = RESEÑA.CLIENTE)
			BEGIN
				INSERT INTO RESEÑA VALUES (@CLIENTE,@RESEÑA,@PUNTOS)
				SET @RESPONSE = 1
			END
			ELSE
				BEGIN
					SET @RESPONSE = 2
				END
		END
END
SELECT @RESPONSE AS RESPONSE
GO


--CONSULTAR RESEÑAS
CREATE OR ALTER PROCEDURE LISTAR_RESEÑAS
AS
SELECT C.NOMBRES, R.RESEÑA, R.PUNTUACION FROM CLIENTE AS C INNER JOIN RESEÑA AS R ON C.ID = R.CLIENTE
GO

--AUTOGENERAR CLIENTE
CREATE OR ALTER PROCEDURE AUTOCLIENT
AS
BEGIN
    SET NOCOUNT ON;

    -- Declaración de variables
    DECLARE @DNI CHAR(8), 
            @NOMBRES VARCHAR(50), 
            @FECHA VARCHAR(20);

    -- Generar un DNI aleatorio de 8 dígitos
    SET @DNI = CAST(10000000 + ABS(CHECKSUM(NEWID())) % 90000000 AS VARCHAR(8));

    -- Generar la fecha actual en formato dd/mm/yy HH:mm:ss
    SET @FECHA = FORMAT(GETDATE(), 'dd/MM/yy HH:mm:ss');

    -- Generar el nombre del cliente
    SET @NOMBRES = 'Cliente autogenerado ' + @FECHA;

    -- Insertar el nuevo cliente
    INSERT INTO CLIENTE (DNI, NOMBRES)
    VALUES (@DNI, @NOMBRES);

    -- Devolver el DNI generado
    SELECT @DNI AS DNI_GENERADO;
END
GO


--INSERCCIONES

--CLIENTES
INSERT INTO CLIENTE (DNI,NOMBRES) VALUES ('83548456','Pablo Díaz')
INSERT INTO CLIENTE (DNI,NOMBRES) VALUES ('65673456','Raul Alutti')
INSERT INTO CLIENTE (DNI,NOMBRES) VALUES ('36673456','Teresa Morales')
INSERT INTO CLIENTE (DNI,NOMBRES) VALUES ('76445236','Franchesca Dicardi')

--RESEÑAS
EXEC MAKE_REVIEW '83548456', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque laoreet justo est, eu laoreet dolor interdum id. In id malesuada sem, vel porta turpis. In faucibus lectus nec arcu facilisis eleifend. Proin tempus est erat, non scelerisque eros pharetra in.',5
GO
EXEC MAKE_REVIEW '65673456', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque laoreet justo est, eu laoreet dolor interdum id. In id malesuada sem, vel porta turpis. In faucibus lectus nec arcu facilisis eleifend. Proin tempus est erat, non scelerisque eros pharetra in.',3
GO
EXEC MAKE_REVIEW '36673456', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque laoreet justo est, eu laoreet dolor interdum id. In id malesuada sem, vel porta turpis. In faucibus lectus nec arcu facilisis eleifend. Proin tempus est erat, non scelerisque eros pharetra in.',4
GO

--PRUEBAS
EXEC LISTAR_RESEÑAS
GO

EXEC AUTOCLIENT
GO

SELECT * FROM CLIENTE
GO

DELETE FROM RESEÑA
GO