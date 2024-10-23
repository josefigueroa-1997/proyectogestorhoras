alter PROCEDURE [dbo].[REPORTEMARGENquarter]
AS
BEGIN
    DECLARE @YearActual INT = YEAR(GETDATE()); -- Año actual

    -- CTE para generar trimestres del año actual
    WITH Trimestres AS (
        SELECT 1 AS Quarter, @YearActual AS Year
        UNION ALL SELECT 2, @YearActual
        UNION ALL SELECT 3, @YearActual
        UNION ALL SELECT 4, @YearActual
    ),
    -- CTE para obtener los montos de gastos y servicios por proyecto
    GastosServicios AS (
        SELECT 
            PG.ID_PROYECTO,
            SUM(PG.MONTO) AS TOTAL_GASTOS
        FROM PROYECTO_GASTOS PG
        GROUP BY PG.ID_PROYECTO
    ),
    Servicios AS (
        SELECT 
            PS.ID_PROYECTO,
            SUM(PS.MONTO) AS TOTAL_SERVICIOS
        FROM PROYECTO_SERVICIO PS
        GROUP BY PS.ID_PROYECTO
    )
    -- Seleccionamos los datos de cada proyecto y calculamos los costos agrupados por trimestre
    SELECT 
        T.Quarter, 
        T.Year,
        COALESCE(P.NUM_PROYECTO, 'Sin Proyecto') AS NUM_PROYECTO,
        COALESCE(P.NOMBRE, 'Sin Proyecto') AS NOMBRE_PROYECTO,
        COALESCE(SUM(PRE.MONTO), 0) AS MONTO_PROYECTO, -- Suma de ingresos por proyecto
        
        -- Suma total de gastos por proyecto
        COALESCE(SUM(GS.TOTAL_GASTOS), 0) AS TOTAL_GASTOS,
             
        -- Suma total de servicios por proyecto
        COALESCE(SUM(SV.TOTAL_SERVICIOS), 0) AS TOTAL_SERVICIOS,
        
        -- Calcular costos por cada tipo de horas y sumarlos
        COALESCE(SUM(HU.HH_SOCIOS * HC.COSTOSOCIO), 0) AS COSTO_SOCIOS,
        COALESCE(SUM(HU.HH_STAFF * HC.COSTOSTAFF), 0) AS COSTO_STAFF,
        COALESCE(SUM(HU.HH_CONSULTORA * HC.COSTOCONSULTORA), 0) AS COSTO_CONSULTORA,
        COALESCE(SUM(HU.HH_CONSULTORB * HC.COSTOCONSULTORB), 0) AS COSTO_CONSULTORB,
        COALESCE(SUM(HU.HH_CONSULTORC * HC.COSTOCONSULTORC), 0) AS COSTO_CONSULTORC,

        -- Cálculo del margen de contribución para cada trimestre
        COALESCE(SUM(PRE.MONTO), 0) - (
            COALESCE(SUM(HU.HH_SOCIOS * HC.COSTOSOCIO), 0) +
            COALESCE(SUM(HU.HH_STAFF * HC.COSTOSTAFF), 0) +
            COALESCE(SUM(HU.HH_CONSULTORA * HC.COSTOCONSULTORA), 0) +
            COALESCE(SUM(HU.HH_CONSULTORB * HC.COSTOCONSULTORB), 0) +
            COALESCE(SUM(HU.HH_CONSULTORC * HC.COSTOCONSULTORC), 0) +
            COALESCE(SUM(GS.TOTAL_GASTOS), 0) +
            COALESCE(SUM(SV.TOTAL_SERVICIOS), 0)
        ) AS MARGEN_DE_CONTRIBUCION
    FROM 
        Trimestres T
    LEFT JOIN PROYECTO P ON P.FECHA_INICIO >= DATEADD(MONTH, (T.Quarter - 1) * 3, CAST(CONCAT(T.Year, '-01-01') AS DATE)) 
                                                  AND P.FECHA_INICIO < DATEADD(MONTH, T.Quarter * 3, CAST(CONCAT(T.Year, '-01-01') AS DATE))
    LEFT JOIN PRESUPUESTO PRE ON PRE.ID = P.ID
    LEFT JOIN HH_USUARIO_HISTORIAL HU ON HU.ID_PROYECTO = P.ID
    LEFT JOIN HISTORIAL_COSTOS_PROYECTOS HC ON HC.IDPROYECTO = P.ID
    LEFT JOIN GastosServicios GS ON GS.ID_PROYECTO = P.ID
    LEFT JOIN Servicios SV ON SV.ID_PROYECTO = P.ID
	
    -- Agrupamos por trimestre, año y datos de costos
    GROUP BY 
        T.Quarter, T.Year, 
        P.NUM_PROYECTO, P.NOMBRE,
        HC.COSTOCONSULTORA, HC.COSTOCONSULTORB, HC.COSTOCONSULTORC,
        HC.COSTOSOCIO, HC.COSTOSTAFF,
        HU.HH_CONSULTORA, HU.HH_CONSULTORB, HU.HH_CONSULTORC, HU.HH_SOCIOS, HU.HH_STAFF
END;
