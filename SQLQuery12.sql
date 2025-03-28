



CREATE TABLE METATIPOLOGIAS(

	ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ANIO INT,
	T1 INT,
	T2 INT,
	T3 INT,
	T4 INT


);

CREATE TABLE TOTALMETATIPOLOGIA(

	ID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
	ANIO INT,
	TOTALPROYECTO INT,
	TOTALPORPROYECTO DECIMAL (15,2),
	DURACIONMEDIA INT,
	TOTALMENSUALES DECIMAL(15,2)


);

ALTER PROCEDURE INSERTARMETATIPOLOGIA
@ANIO INT,
@T1 INT,
@T2 INT,
@T3 INT,
@T4 INT
AS
BEGIN
DECLARE @TOTALPROYECTO INT;
DECLARE @TOTALANIOFACTURA DECIMAL(15,2);
DECLARE @TOTALPORPROYECTO DECIMAL(15,2);
SELECT @TOTALANIOFACTURA=MONTO FROM TOTALQUARTERFACTURAANIO WHERE ANIO=@ANIO
DECLARE @PRODUCTOTIPOLOGIAS INT;
DECLARE @DURACIONMEDIA INT;
DECLARE @TOTALMENSUALES DECIMAL(15,2);


SET @TOTALPROYECTO = @T1 + @T2 + @T3 + @T4;

SET @TOTALPORPROYECTO=ROUND(@TOTALANIOFACTURA/@TOTALPROYECTO,-6)
SET @PRODUCTOTIPOLOGIAS = @T1*8 + @T2*6 + @T3*3 + @T4
SET @DURACIONMEDIA = ROUND(@PRODUCTOTIPOLOGIAS / @TOTALPROYECTO, 2)
SET @TOTALMENSUALES = ROUND(@TOTALPORPROYECTO/@DURACIONMEDIA,-6);


INSERT INTO METATIPOLOGIAS(ANIO,T1,T2,T3,T4) VALUES(@ANIO,@T1,@T2,@T3,@T4)


INSERT INTO TOTALMETATIPOLOGIA(ANIO,TOTALPROYECTO,TOTALPORPROYECTO,DURACIONMEDIA,TOTALMENSUALES)
VALUES (@ANIO,@TOTALPROYECTO,@TOTALPORPROYECTO,@DURACIONMEDIA,@TOTALMENSUALES)


END;


DELETE FROM METATIPOLOGIAS
DELETE FROM TOTALMETATIPOLOGIA

exec INSERTARMETATIPOLOGIA 2024,2,3,4,8

SELECT * FROM METATIPOLOGIAS

SELECT * FROM TOTALMETATIPOLOGIA