ALTER PROCEDURE sp_HorasSociosPorActividad
    @Mes INT -- El mes que se ingresa como parámetro (1-12)
AS
BEGIN
    -- Variable para el año actual
    DECLARE @AnoActual INT = YEAR(GETDATE());
    
    -- Variable para la fecha inicial del año actual (1 de enero)
    DECLARE @FechaInicioAnoActual DATE = CONVERT(DATE, CONCAT(@AnoActual, '-01-01'));
    
    -- Variable para la fecha actual del mes ingresado (último día del mes)
    DECLARE @FechaFinMesIngresado DATE = EOMONTH(CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01')));
    
    -- Variable para el primer día del mes ingresado
    DECLARE @FechaInicioMesIngresado DATE = CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01'));
    
    -- Variable para la fecha hace 12 meses desde el mes ingresado
    DECLARE @FechaInicioUltimoAno DATE = DATEADD(MONTH, -11, @FechaInicioMesIngresado);

    -- Consultar las horas para socios en el mes actual, incluyendo actividades sin horas registradas
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesMesActual
    FROM Actividades a
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.ACTIVIDAD = a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioMesIngresado AND @FechaFinMesIngresado
    LEFT JOIN USUARIO_PROYECTO up ON pu.ID_USU_PROY = up.Id
    LEFT JOIN Usuario u ON up.ID_USUARIO = u.Id
    LEFT JOIN Recurso r ON u.ID_RECURSO = r.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' OR r.NOMBRE_RECURSO IS NULL
    GROUP BY r.NOMBRE_RECURSO, a.Nombre, u.Nombre;

    -- Consultar las horas acumuladas desde enero del año actual hasta el mes ingresado
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesAnoActual
    FROM Actividades a
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.ACTIVIDAD = a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioAnoActual AND @FechaFinMesIngresado
    LEFT JOIN USUARIO_PROYECTO up ON pu.ID_USU_PROY = up.Id
    LEFT JOIN Usuario u ON up.ID_USUARIO = u.Id
    LEFT JOIN Recurso r ON u.ID_RECURSO = r.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' OR r.NOMBRE_RECURSO IS NULL
    GROUP BY r.NOMBRE_RECURSO, a.Nombre, u.Nombre;

    -- Consultar las horas acumuladas en los últimos 12 meses
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesUltimoAno
    FROM Actividades a
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.ACTIVIDAD = a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioUltimoAno AND @FechaFinMesIngresado
    LEFT JOIN USUARIO_PROYECTO up ON pu.ID_USU_PROY = up.Id
    LEFT JOIN Usuario u ON up.ID_USUARIO = u.Id
    LEFT JOIN Recurso r ON u.ID_RECURSO = r.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' OR r.NOMBRE_RECURSO IS NULL
    GROUP BY r.NOMBRE_RECURSO, a.Nombre, u.Nombre;
END;




ALTER PROCEDURE sp_HorasSociosPorActividadMESACTUAL
    @Mes INT -- El mes que se ingresa como parámetro (1-12)
AS
BEGIN
    -- Variable para el año actual
    DECLARE @AnoActual INT = YEAR(GETDATE());
    
    -- Variable para la fecha inicial del año actual (1 de enero)
    DECLARE @FechaInicioAnoActual DATE = CONVERT(DATE, CONCAT(@AnoActual, '-01-01'));
    
    -- Variable para la fecha actual del mes ingresado (último día del mes)
    DECLARE @FechaFinMesIngresado DATE = EOMONTH(CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01')));
    
    -- Variable para el primer día del mes ingresado
    DECLARE @FechaInicioMesIngresado DATE = CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01'));
    
    -- Consultar las horas para socios en el mes actual, incluyendo actividades sin horas registradas
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesMesActual
    FROM Recurso r
    LEFT JOIN Usuario u ON r.Id = u.Id_Recurso
    LEFT JOIN USUARIO_PROYECTO up ON u.Id = up.ID_USUARIO
    LEFT JOIN ACTIVIDADES a ON 1 = 1 -- Combina con todas las actividades
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.id_actividad= a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioMesIngresado AND @FechaFinMesIngresado
        AND pu.ID_USU_PROY = up.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' AND A.TIPO_ACATIVIDAD = 'Socio'
    GROUP BY r.Nombre_Recurso, u.Nombre, a.Nombre,a.id
    order by a.id;




END;


CREATE PROCEDURE sp_HorasSociosPorActividadANIOACTUAL

  @Mes INT -- El mes que se ingresa como parámetro (1-12)
AS
BEGIN
    -- Variable para el año actual
    DECLARE @AnoActual INT = YEAR(GETDATE());
    
    -- Variable para la fecha inicial del año actual (1 de enero)
    DECLARE @FechaInicioAnoActual DATE = CONVERT(DATE, CONCAT(@AnoActual, '-01-01'));
    
    -- Variable para la fecha actual del mes ingresado (último día del mes)
    DECLARE @FechaFinMesIngresado DATE = EOMONTH(CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01')));
    
    -- Variable para el primer día del mes ingresado
    DECLARE @FechaInicioMesIngresado DATE = CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01'));
    
    -- Consultar las horas para socios en el mes actual, incluyendo actividades sin horas registradas
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesMesActual
    FROM Recurso r
    LEFT JOIN Usuario u ON r.Id = u.Id_Recurso
    LEFT JOIN USUARIO_PROYECTO up ON u.Id = up.ID_USUARIO
    LEFT JOIN ACTIVIDADES a ON 1 = 1 -- Combina con todas las actividades
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.ACTIVIDAD = a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioAnoActual AND @FechaFinMesIngresado
        AND pu.ID_USU_PROY = up.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' AND A.TIPO_ACATIVIDAD = 'Socio'
    GROUP BY r.Nombre_Recurso, u.Nombre, a.Nombre,a.id
	order by a.id
    
END;








CREATE PROCEDURE sp_HorasSociosPorActividad12meses

  @Mes INT -- El mes que se ingresa como parámetro (1-12)
AS
BEGIN
    -- Variable para el año actual
    DECLARE @AnoActual INT = YEAR(GETDATE());
    
    -- Variable para la fecha inicial del año actual (1 de enero)
    DECLARE @FechaInicioAnoActual DATE = CONVERT(DATE, CONCAT(@AnoActual, '-01-01'));
    
    -- Variable para la fecha actual del mes ingresado (último día del mes)
    DECLARE @FechaFinMesIngresado DATE = EOMONTH(CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01')));
    
    -- Variable para el primer día del mes ingresado
    DECLARE @FechaInicioMesIngresado DATE = CONVERT(DATE, CONCAT(@AnoActual, '-', @Mes, '-01'));

	 -- Variable para la fecha hace 12 meses desde el mes ingresado
    DECLARE @FechaInicioUltimoAno DATE = DATEADD(MONTH, -11, @FechaInicioMesIngresado);
    
    -- Consultar las horas para socios en el mes actual, incluyendo actividades sin horas registradas
    SELECT 
        r.Nombre_Recurso AS Recurso,
        u.Nombre AS NombreUsuario,
        a.Nombre AS Actividad,
        ISNULL(SUM(pu.REGISTRO_HH_PROYECTO), 0) AS HorasTotalesMesActual
    FROM Recurso r
    LEFT JOIN Usuario u ON r.Id = u.Id_Recurso
    LEFT JOIN USUARIO_PROYECTO up ON u.Id = up.ID_USUARIO
    LEFT JOIN ACTIVIDADES a ON 1 = 1 -- Combina con todas las actividades
    LEFT JOIN PLANILLA_USUSARIO_PROYECTO pu ON pu.ACTIVIDAD = a.Id 
        AND pu.FECHA_REGISTRO BETWEEN @FechaInicioUltimoAno AND @FechaFinMesIngresado
        AND pu.ID_USU_PROY = up.Id
    WHERE r.NOMBRE_RECURSO = 'Socio' AND A.TIPO_ACATIVIDAD = 'Socio'
    GROUP BY r.Nombre_Recurso, u.Nombre, a.Nombre,a.id
	order by a.id
    
END;


ALTER PROCEDURE [dbo].[REPORTENEOGOCIACION]
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #NegocioanTemp (
        Num_Proyecto VARCHAR(100),
        Nombre VARCHAR(100),
        Estado VARCHAR(100),
        Tipologia VARCHAR(100),
        NombreCliente VARCHAR(200),
        Probabilidad VARCHAR(200),
        UnidadNegocio VARCHAR(200),
        CentroCosto VARCHAR(200),
        FechaFactura DATE,
        Monto DECIMAL(18, 2),
        MontoIVA DECIMAL(18, 2),
        Total DECIMAL(18, 2)
    );

    -- CTE para obtener la última negociación por proyecto
    WITH UltimaNegociacion AS (
        SELECT 
            HC.ID_PROYECTO,
            HC.PROBABILIDAD,
            HC.FECHANEGOCIACION,
			HC.FECHAINICIO,
			HC.PLAZO,
			HC.MONTO,
			HC.AFECTAIVA,
            ROW_NUMBER() OVER (PARTITION BY HC.ID_PROYECTO ORDER BY HC.ID DESC) AS rn
        FROM 
            HISTORIAL_NEGOCIACION HC
    )
    
    INSERT INTO #NegocioanTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, Probabilidad, UnidadNegocio, CentroCosto, FechaFactura, Monto, MontoIVA, Total)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        hc.PROBABILIDAD,
        UN.TIPO_UNEGOCIO,
        CC.TIPO_CCOSTO,
        DATEADD(MONTH, m.MonthNumber + 1, hc.FECHAINICIO) AS FechaFactura, -- Ajustar para que empiece un mes después
        ROUND(HC.MONTO / HC.PLAZO, 2) AS Monto,
        CASE 
            WHEN HC.AFECTAIVA = 'SI' THEN ROUND(ROUND(HC.MONTO / HC.PLAZO, 2) * 0.19, 2)
            ELSE 0
        END AS MontoIVA,
        ROUND(
            ROUND(HC.MONTO / HC.PLAZO, 2) + 
            CASE 
                WHEN HC.AFECTAIVA = 'SI' THEN ROUND(ROUND(HC.MONTO / HC.PLAZO, 2) * 0.19, 2)
                ELSE 0
            END, 2
        ) AS Total
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
        CCOSTO_UNEGOCIO CU ON CU.ID = P.ID_CCOSTO_UNEGOCIO
    INNER JOIN 
        UNEGOCIO UN ON UN.ID = CU.ID_UNEGOCIO
    INNER JOIN 
        CCOSTO CC ON CC.ID = CU.ID_CCOSTO
    INNER JOIN 
        UltimaNegociacion HC ON HC.ID_PROYECTO = P.ID AND HC.rn = 1
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < HC.PLAZO  AND P.STATUS_PROYECTO=1;

    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        Probabilidad AS 'Probabilidad',
        UnidadNegocio AS 'Unegocio',
        CentroCosto AS 'CCosto',
        FechaFactura AS 'Fecha_Factura', 
        ROUND(Monto, 2) AS 'Neto',
        ROUND(MontoIVA, 2) AS 'IVA',
        ROUND(Total, 2) AS 'Total'
    FROM 
        #NegocioanTemp;

    DROP TABLE #NegocioanTemp;

    SET NOCOUNT OFF;
END;
