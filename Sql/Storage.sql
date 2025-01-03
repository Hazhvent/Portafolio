--CREACION DE LA BASE DE DATOS
IF NOT EXISTS (SELECT *FROM sys.databases WHERE name = 'STORAGE')
        CREATE DATABASE [STORAGE]            
GO

--MONTAR BASE DE DATOS
USE STORAGE
GO

--TABLAS

--GENERO
IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='Genero')
CREATE TABLE Genero (
Id	            TINYINT IDENTITY (1,1) PRIMARY KEY,
Descripcion		VARCHAR(20) NOT NULL
)
GO

--CLASIFICACION
IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='Clasificacion')
CREATE TABLE Clasificacion (
Id	            TINYINT IDENTITY (1,1) PRIMARY KEY,
Descripcion		VARCHAR(20) NOT NULL
)
GO

--PELICULA
IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='Pelicula')
CREATE TABLE Pelicula (
Id	            INT IDENTITY (1,1) PRIMARY KEY,
Nombre		    VARCHAR(50) NOT NULL,
Genero          TINYINT NOT NULL,
Clasificacion   TINYINT NOT NULL,
Version         TINYINT NOT NULL,
Path            VARCHAR(50) NOT NULL,
Estado          BIT NOT NULL

FOREIGN KEY (Genero) REFERENCES Genero,
FOREIGN KEY (Clasificacion) REFERENCES Clasificacion
)
GO

--GENEROS
INSERT INTO Genero (Descripcion) VALUES ('Apocaliptico')
INSERT INTO Genero (Descripcion) VALUES ('Fantasia')
INSERT INTO Genero (Descripcion) VALUES ('Terror')
INSERT INTO Genero (Descripcion) VALUES ('Thriller')
INSERT INTO Genero (Descripcion) VALUES ('Romance')
INSERT INTO Genero (Descripcion) VALUES ('Anime')
GO
--CLASIFICACION
INSERT INTO Clasificacion (Descripcion) VALUES ('Todos')
INSERT INTO Clasificacion (Descripcion) VALUES ('Adolescentes')
INSERT INTO Clasificacion (Descripcion) VALUES ('Adultos')
GO	

-- PELICULAS
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('One Day', 5, 2, 1, '/attached/img04/1.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Ciudad de Angeles', 5, 2, 1,'/attached/img04/2.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Halloween', 3, 3, 0,'/attached/img04/3.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Latherface', 3, 3, 0,'/attached/img04/4.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Hobbit 1', 2, 1, 1,'/attached/img04/5.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Hobbit 2', 2, 1, 1,'/attached/img04/6.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Hobbit 3', 2, 1, 1,'/attached/img04/7.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('World Warz', 1, 2, 0,'/attached/img04/8.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('28 Days Later', 1, 2, 1,'/attached/img04/9.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('El perfume', 4, 3, 0,'/attached/img04/10.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Far and Away', 5, 2, 0,'/attached/img04/11.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Clannad', 6, 2, 0,'/attached/img04/12.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Final Fantasy VII', 6, 2, 1,'/attached/img04/13.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Remember Me', 5, 2, 0,'/attached/img04/14.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Eternal Sunshine', 5, 2, 0,'/attached/img04/15.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('LOFR 1', 2, 1, 1,'/attached/img04/16.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('LOFR 2', 2, 1, 1,'/attached/img04/17.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('LOFR 3', 2, 1, 1,'/attached/img04/18.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('28 Weeks Later', 1, 3, 1,'/attached/img04/19.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Dorian Gray', 4, 3, 1,'/attached/img04/20.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('The invitation', 4, 3, 0,'/attached/img04/21.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Barbarian', 3, 3, 0,'/attached/img04/22.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Ninja Scroll', 6, 3, 1,'/attached/img04/23.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Perfect Blue', 6, 3, 0,'/attached/img04/24.jpg', 1);
INSERT INTO Pelicula (Nombre, Genero, Clasificacion, Version, Path, Estado) VALUES ('Ghost in the Shell', 6, 3, 1,'/attached/img04/25.jpg', 1);
GO


SELECT * FROM Pelicula