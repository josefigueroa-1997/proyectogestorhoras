ALTER PROCEDURE REPORTEMARGEN
AS
BEGIN
    -- Seleccionamos los datos de cada proyecto y calculamos los costos
    SELECT 
        P.NUM_PROYECTO AS NUM_PROYECTO,
        P.NOMBRE AS NOMBRE_PROYECTO,
        PRE.MONTO AS MONTO_PROYECTO,
        
        -- Subconsulta para obtener el total de gastos por proyecto
        ISNULL(
            (SELECT SUM(PG.MONTO) 
             FROM PROYECTO_GASTOS PG 
             WHERE PG.ID_PROYECTO = P.ID), 0) AS TOTAL_GASTOS,
             
        -- Subconsulta para obtener el total de servicios por proyecto
        ISNULL(
            (SELECT SUM(PS.MONTO) 
             FROM PROYECTO_SERVICIO PS 
             WHERE PS.ID_PROYECTO = P.ID), 0) AS TOTAL_SERVICIOS,
        
        -- Calcular costos por cada tipo de horas
        ISNULL((HU.HH_SOCIOS * HC.COSTOSOCIO), 0) AS COSTO_SOCIOS,
        ISNULL((HU.HH_STAFF * HC.COSTOSTAFF), 0) AS COSTO_STAFF,
        ISNULL((HU.HH_CONSULTORA * HC.COSTOCONSULTORA), 0) AS COSTO_CONSULTORA,
        ISNULL((HU.HH_CONSULTORB * HC.COSTOCONSULTORB), 0) AS COSTO_CONSULTORB,
        ISNULL((HU.HH_CONSULTORC * HC.COSTOCONSULTORC), 0) AS COSTO_CONSULTORC,

        -- Cálculo del margen de contribución para cada proyecto
        PRE.MONTO - (
            ISNULL((HU.HH_SOCIOS * HC.COSTOSOCIO), 0) +
            ISNULL((HU.HH_STAFF * HC.COSTOSTAFF), 0) +
            ISNULL((HU.HH_CONSULTORA * HC.COSTOCONSULTORA), 0) +
            ISNULL((HU.HH_CONSULTORB * HC.COSTOCONSULTORB), 0) +
            ISNULL((HU.HH_CONSULTORC * HC.COSTOCONSULTORC), 0) +
            ISNULL(
                (SELECT SUM(PG.MONTO) 
                 FROM PROYECTO_GASTOS PG 
                 WHERE PG.ID_PROYECTO = P.ID), 0) +
            ISNULL(
                (SELECT SUM(PS.MONTO) 
                 FROM PROYECTO_SERVICIO PS 
                 WHERE PS.ID_PROYECTO = P.ID), 0)
        ) AS MARGEN_DE_CONTRIBUCION
    FROM 
        PROYECTO P
    LEFT JOIN PRESUPUESTO PRE ON PRE.ID = P.ID
    LEFT JOIN HH_USUARIO_HISTORIAL HU ON HU.ID_PROYECTO = P.ID
    LEFT JOIN HISTORIAL_COSTOS_PROYECTOS HC ON HC.IDPROYECTO = P.ID
    -- Agrupamos por proyecto y datos de costos
    GROUP BY 
        P.NUM_PROYECTO, P.NOMBRE, PRE.MONTO,
        HC.COSTOCONSULTORA, HC.COSTOCONSULTORB, HC.COSTOCONSULTORC,
        HC.COSTOSOCIO, HC.COSTOSTAFF,
        HU.HH_CONSULTORA, HU.HH_CONSULTORB, HU.HH_CONSULTORC, HU.HH_SOCIOS, HU.HH_STAFF,P.ID
END;






alter PROCEDURE [dbo].[REPORTECOSTOCONSULTORES]
@TIPOCONSULTOR VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #FacturasTemp (
        Num_Proyecto VARCHAR(100),
        Nombre VARCHAR(100),
        Estado VARCHAR(100),
        Tipologia VARCHAR(100),
        NombreCliente VARCHAR(200),
        FechaFactura DATE,
        CostoRecurso DECIMAL(10, 2)  -- Cambiado a DECIMAL para permitir decimales
    );

    -- Obtener los proyectos y sus detalles necesarios para las facturas
    INSERT INTO #FacturasTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, FechaFactura, CostoRecurso)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        DATEADD(MONTH, m.MonthNumber + 1, P.FECHA_INICIO) AS FechaFactura,
        CASE 
            WHEN @TIPOCONSULTOR = 'Consultor A' THEN CAST(H.HH_CONSULTORA * HC.COSTOCONSULTORA AS DECIMAL(10, 2)) / P.PLAZO
            WHEN @TIPOCONSULTOR = 'Consultor B' THEN CAST(H.HH_CONSULTORB * HC.COSTOCONSULTORB AS DECIMAL(10, 2)) / P.PLAZO
			WHEN @TIPOCONSULTOR = 'Consultor C' THEN CAST(H.HH_CONSULTORC * HC.COSTOCONSULTORC AS DECIMAL(10, 2)) / P.PLAZO
        END AS CostoRecurso
    FROM 
        PROYECTO P
    INNER JOIN 
        PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
    INNER JOIN 
        STATUS_PROYECTO SP ON SP.ID = P.STATUS_PROYECTO
    INNER JOIN 
        TIPOLOGIA T ON T.ID = P.ID_TIPOLOGIA
    INNER JOIN 
        SUCURSAL_CLIENTE SC ON SC.ID = P.ID_CLIENTE_SUCURSAL
    INNER JOIN 
        CLIENTE C ON C.ID = SC.ID_CLIENTE
    INNER JOIN 
        HH_USUARIO_HISTORIAL H ON P.ID = H.ID_PROYECTO
	INNER JOIN
		HISTORIAL_COSTOS_PROYECTOS HC ON HC.IDPROYECTO = P.ID
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < P.PLAZO AND SP.id<>4 AND SP.id<>3;

    -- Mostrar las facturas generadas
    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        FechaFactura AS 'Fecha_Factura', 
        CostoRecurso AS 'COSTO_Recurso'
    FROM 
        #FacturasTemp;

    DROP TABLE #FacturasTemp;

    SET NOCOUNT OFF;
END;


ALTER PROCEDURE [dbo].[REPORTECOSTOSOCIOSSTAFF]
@NOMBRERECURSO VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #FacturasTemp (
        Num_Proyecto VARCHAR(100),
        Nombre VARCHAR(100),
        Estado VARCHAR(100),
        Tipologia VARCHAR(100),
        NombreCliente VARCHAR(200),
        FechaFactura DATE,
        COSTORECURSO DECIMAL(10, 2)  -- Cambiado a DECIMAL para permitir decimales
    );

    -- Obtener los proyectos y sus detalles necesarios para las facturas
    INSERT INTO #FacturasTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, FechaFactura, COSTORECURSO)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        DATEADD(MONTH, m.MonthNumber + 1, P.FECHA_INICIO) AS FechaFactura,
        CASE 
            WHEN @NOMBRERECURSO = 'Socio' THEN CAST(H.HH_SOCIOS * HC.COSTOSOCIO AS DECIMAL(10, 2)) / P.PLAZO
            WHEN @NOMBRERECURSO = 'Staff' THEN CAST(H.HH_STAFF * HC.COSTOSTAFF AS DECIMAL(10, 2)) / P.PLAZO
        END AS COSTORECURSO
    FROM 
        PROYECTO P
    INNER JOIN 
        PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
    INNER JOIN 
        STATUS_PROYECTO SP ON SP.ID = P.STATUS_PROYECTO
    INNER JOIN 
        TIPOLOGIA T ON T.ID = P.ID_TIPOLOGIA
    INNER JOIN 
        SUCURSAL_CLIENTE SC ON SC.ID = P.ID_CLIENTE_SUCURSAL
    INNER JOIN 
        CLIENTE C ON C.ID = SC.ID_CLIENTE
    INNER JOIN 
        HH_USUARIO_HISTORIAL H ON P.ID = H.ID_PROYECTO
	INNER JOIN
		HISTORIAL_COSTOS_PROYECTOS HC ON P.ID = HC.IDPROYECTO
	
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < P.PLAZO AND SP.id<>4 AND SP.id<>3;

    -- Mostrar las facturas generadas
    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        FechaFactura AS 'Fecha_Factura', 
        COSTORECURSO AS 'COSTO_Recurso'
    FROM 
        #FacturasTemp;

    DROP TABLE #FacturasTemp;

    SET NOCOUNT OFF;
END;


alter PROCEDURE [dbo].[REPORTEHHCONSULTORES]
@TIPOCONSULTOR VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #FacturasTemp (
        Num_Proyecto VARCHAR(100),
        Nombre VARCHAR(100),
        Estado VARCHAR(100),
        Tipologia VARCHAR(100),
        NombreCliente VARCHAR(200),
        FechaFactura DATE,
        HHRECURSO DECIMAL(10, 2)  -- Cambiado a DECIMAL para permitir decimales
    );

    -- Obtener los proyectos y sus detalles necesarios para las facturas
    INSERT INTO #FacturasTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, FechaFactura, HHRECURSO)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        DATEADD(MONTH, m.MonthNumber + 1, P.FECHA_INICIO) AS FechaFactura,
        CASE 
            WHEN @TIPOCONSULTOR = 'Consultor A' THEN CAST(H.HH_CONSULTORA AS DECIMAL(10, 2)) / P.PLAZO
            WHEN @TIPOCONSULTOR = 'Consultor B' THEN CAST(H.HH_CONSULTORB AS DECIMAL(10, 2)) / P.PLAZO
			WHEN @TIPOCONSULTOR = 'Consultor C' THEN CAST(H.HH_CONSULTORC AS DECIMAL(10, 2)) / P.PLAZO
        END AS HHRECURSO
    FROM 
        PROYECTO P
    INNER JOIN 
        PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
    INNER JOIN 
        STATUS_PROYECTO SP ON SP.ID = P.STATUS_PROYECTO
    INNER JOIN 
        TIPOLOGIA T ON T.ID = P.ID_TIPOLOGIA
    INNER JOIN 
        SUCURSAL_CLIENTE SC ON SC.ID = P.ID_CLIENTE_SUCURSAL
    INNER JOIN 
        CLIENTE C ON C.ID = SC.ID_CLIENTE
    INNER JOIN 
        HH_USUARIO_HISTORIAL H ON P.ID = H.ID_PROYECTO -- Aquí se hace el join
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < P.PLAZO AND SP.id<>4 AND SP.id<>3;

    -- Mostrar las facturas generadas
    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        FechaFactura AS 'Fecha_Factura', 
        HHRECURSO AS 'HH_Recurso'
    FROM 
        #FacturasTemp;

    DROP TABLE #FacturasTemp;

    SET NOCOUNT OFF;
END;



ALTER PROCEDURE [dbo].[REPORTEHHSOCIOSSTAFF]
@NOMBRERECURSO VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #FacturasTemp (
        Num_Proyecto VARCHAR(100),
        Nombre VARCHAR(100),
        Estado VARCHAR(100),
        Tipologia VARCHAR(100),
        NombreCliente VARCHAR(200),
        FechaFactura DATE,
        HHRECURSO DECIMAL(10, 2)  -- Cambiado a DECIMAL para permitir decimales
    );

    -- Obtener los proyectos y sus detalles necesarios para las facturas
    INSERT INTO #FacturasTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, FechaFactura, HHRECURSO)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        DATEADD(MONTH, m.MonthNumber + 1, P.FECHA_INICIO) AS FechaFactura,
        CASE 
            WHEN @NOMBRERECURSO = 'Socio' THEN CAST(H.HH_SOCIOS AS DECIMAL(10, 2)) / P.PLAZO
            WHEN @NOMBRERECURSO = 'Staff' THEN CAST(H.HH_STAFF AS DECIMAL(10, 2)) / P.PLAZO
        END AS HHRECURSO
    FROM 
        PROYECTO P
    INNER JOIN 
        PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
    INNER JOIN 
        STATUS_PROYECTO SP ON SP.ID = P.STATUS_PROYECTO
    INNER JOIN 
        TIPOLOGIA T ON T.ID = P.ID_TIPOLOGIA
    INNER JOIN 
        SUCURSAL_CLIENTE SC ON SC.ID = P.ID_CLIENTE_SUCURSAL
    INNER JOIN 
        CLIENTE C ON C.ID = SC.ID_CLIENTE
    INNER JOIN 
        HH_USUARIO_HISTORIAL H ON P.ID = H.ID_PROYECTO -- Aquí se hace el join
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < P.PLAZO AND SP.id<>4 AND SP.id<>3;

    -- Mostrar las facturas generadas
    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        FechaFactura AS 'Fecha_Factura', 
        HHRECURSO AS 'HH_Recurso'
    FROM 
        #FacturasTemp;

    DROP TABLE #FacturasTemp;

    SET NOCOUNT OFF;
END;