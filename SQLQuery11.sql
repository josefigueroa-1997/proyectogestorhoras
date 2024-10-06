

ALTER PROCEDURE OBTENERSERVICIOSPROYECTO
@IDPROYECTO INT
AS
BEGIN

SELECT S.NOMBRE AS NOMBRESERVICIO,SE.NOMBRE AS NOMBRSEGMENTO,C.CUENTA  ,C.IDCUENTA,PS.MONTO FROM PROYECTO P

INNER JOIN PROYECTO_SERVICIO PS ON P.ID = PS.ID_PROYECTO
INNER JOIN SERVICIOS S ON S.ID = PS.ID_SERVICIO
INNER JOIN SEGMENTO SE ON SE.ID = PS.IDSEGMENTO
INNER JOIN CUENTA C ON C.ID = SE.ID_CUENTA
WHERE P.ID = @IDPROYECTO
END;




create PROCEDURE OBTENERGASTOSPROYECTO
@IDPROYECTO INT
AS
BEGIN

SELECT G.NOMBRE AS NOMBREGASTO,SE.NOMBRE AS NOMBRSEGMENTO,C.CUENTA  ,C.IDCUENTA,PG.MONTO FROM PROYECTO P

INNER JOIN PROYECTO_GASTOS PG ON P.ID = PG.ID_PROYECTO
INNER JOIN GASTOS G ON G.ID = PG.ID_GASTOS
INNER JOIN SEGMENTO SE ON SE.ID = PG.IDSEGMENTO
INNER JOIN CUENTA C ON C.ID = SE.ID_CUENTA
WHERE P.ID = @IDPROYECTO
END;



alter PROCEDURE GENERARFACTURAPROYECTO
    @IDPROYECTO INT
AS
BEGIN
    -- Evita mostrar el conteo de filas afectadas
    SET NOCOUNT ON;

    -- Declarar las variables necesarias
    DECLARE @PLAZO INT;
    DECLARE @AFECTAIVA VARCHAR(100);
    DECLARE @MONTO DECIMAL(10, 2);
    DECLARE @FECHAINICIO DATE;
    DECLARE @VALORIVA DECIMAL(10, 2);
    DECLARE @MONTO_MENSUAL DECIMAL(10, 2);
    DECLARE @MONTO_CON_IVA DECIMAL(10, 2);
    DECLARE @FECHAFACTURA DATE;
    DECLARE @MES INT = 1;

    -- Obtener la información del proyecto usando @IDPROYECTO
    SELECT @PLAZO = P.PLAZO,
           @FECHAINICIO = P.FECHA_INICIO,
           @MONTO = PE.MONTO,
           @AFECTAIVA = PE.AFECTAIVA,
           @VALORIVA = 0.19 -- IVA del 19%
    FROM PROYECTO P
    INNER JOIN PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
    WHERE P.ID = @IDPROYECTO; -- Filtro para el proyecto específico

    -- Calcular el monto mensual
    SET @MONTO_MENSUAL = @MONTO / @PLAZO;

    -- Crear una tabla temporal para almacenar los resultados
    CREATE TABLE #FacturasTemp (
        FechaFactura DATE,
        Monto DECIMAL(10, 2),
        MontoIVA DECIMAL(10, 2),
        Total DECIMAL(10, 2)
    );

    -- Generar facturas para cada mes y guardarlas en la tabla temporal
    WHILE @MES <= @PLAZO
    BEGIN
        -- Calcular la fecha de la factura (un mes después de la fecha de inicio)
        SET @FECHAFACTURA = DATEADD(MONTH, @MES, @FECHAINICIO);

        -- Calcular el monto con IVA si aplica
        IF @AFECTAIVA = 'SI'
        BEGIN
            SET @MONTO_CON_IVA = @MONTO_MENSUAL * (1 + @VALORIVA);
        END
        ELSE
        BEGIN
            SET @MONTO_CON_IVA = @MONTO_MENSUAL;
        END

        -- Insertar los datos en la tabla temporal
        INSERT INTO #FacturasTemp (FechaFactura, Monto, MontoIVA, Total)
        VALUES (@FECHAFACTURA, @MONTO_MENSUAL, @MONTO_CON_IVA - @MONTO_MENSUAL, @MONTO_CON_IVA);

        -- Incrementar el mes
        SET @MES = @MES + 1;
    END

    -- Mostrar las facturas generadas
    SELECT FechaFactura AS 'Fecha Factura', 
           Monto AS 'Monto sin IVA', 
           MontoIVA AS 'Monto IVA', 
           Total AS 'Total con IVA'
    FROM #FacturasTemp;

    -- Eliminar la tabla temporal
    DROP TABLE #FacturasTemp;

    -- Restablecer el comportamiento normal si es necesario
    SET NOCOUNT OFF;
END
