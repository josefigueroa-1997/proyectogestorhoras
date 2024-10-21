
ALTER PROCEDURE ObtenerHorasPoryejecutadasProyecto
    @ProyectoId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarar variables para las fechas de inicio, fin del proyecto y el plazo
    DECLARE @FechaInicio DATETIME;
    DECLARE @FechaFin DATETIME;
    DECLARE @Plazo INT;

    -- Obtener las fechas de inicio, fin del proyecto y el plazo en meses
    SELECT 
        @FechaInicio = FECHA_INICIO, 
        @FechaFin = FECHA_TERMINO,
        @Plazo = DATEDIFF(MONTH, FECHA_INICIO, FECHA_TERMINO)  -- Datediff cuenta el mes de inicio
    FROM 
        Proyecto 
    WHERE 
        Id = @ProyectoId;

    -- Crear una tabla temporal para almacenar los resultados
    CREATE TABLE #HorasPorMes (
        Mes INT,
        Anio INT,
        HorasSociosEjecutadas DECIMAL(18, 2) DEFAULT 0,
        HorasStaffEjecutadas DECIMAL(18, 2) DEFAULT 0,
        HorasConsultorAEjecutadas DECIMAL(10, 2) DEFAULT 0,
        HorasConsultorBEjecutadas DECIMAL(10, 2) DEFAULT 0,
        HorasConsultorCEjecutadas DECIMAL(10, 2) DEFAULT 0,
        HorasSociosPlaneadas DECIMAL(18, 2) DEFAULT 0,
        HorasStaffPlaneadas DECIMAL(18, 2) DEFAULT 0,
        HorasConsultorAPlaneadas DECIMAL(10, 2) DEFAULT 0,
        HorasConsultorBPlaneadas DECIMAL(10, 2) DEFAULT 0,
        HorasConsultorCPlaneadas DECIMAL(10, 2) DEFAULT 0
    );

    -- Insertar las horas ejecutadas
    INSERT INTO #HorasPorMes (Mes, Anio, HorasSociosEjecutadas, HorasStaffEjecutadas, HorasConsultorAEjecutadas, HorasConsultorBEjecutadas, HorasConsultorCEjecutadas)
    SELECT 
        MONTH(PP.FECHA_REGISTRO) AS Mes,
        YEAR(PP.FECHA_REGISTRO) AS Anio,
        SUM(CASE WHEN R.NOMBRE_RECURSO = 'socio' THEN PP.REGISTRO_HH_PROYECTO ELSE 0 END) AS HorasSociosEjecutadas,
        SUM(CASE WHEN R.NOMBRE_RECURSO = 'staff' THEN PP.REGISTRO_HH_PROYECTO ELSE 0 END) AS HorasStaffEjecutadas,
        SUM(CASE WHEN R.TIPO_CONSULTOR = 'Consultor A' THEN PP.REGISTRO_HH_PROYECTO ELSE 0 END) AS HorasConsultorAEjecutadas,
        SUM(CASE WHEN R.TIPO_CONSULTOR = 'Consultor B' THEN PP.REGISTRO_HH_PROYECTO ELSE 0 END) AS HorasConsultorBEjecutadas,
        SUM(CASE WHEN R.TIPO_CONSULTOR = 'Consultor C' THEN PP.REGISTRO_HH_PROYECTO ELSE 0 END) AS HorasConsultorCEjecutadas
    FROM 
        PLANILLA_USUSARIO_PROYECTO PP
    JOIN 
        USUARIO_PROYECTO UP ON PP.ID_USU_PROY = UP.Id
    JOIN 
        Usuario U ON UP.ID_USUARIO = U.Id
    JOIN 
        Recurso R ON U.ID_RECURSO = R.Id
    WHERE 
        UP.ID_PROYECTO = @ProyectoId
        AND PP.FECHA_REGISTRO BETWEEN @FechaInicio AND @FechaFin
    GROUP BY 
        MONTH(PP.FECHA_REGISTRO), 
        YEAR(PP.FECHA_REGISTRO);

    -- Insertar las horas planeadas
    DECLARE @Mes INT = 0;

    -- Cambiar el límite de @Plazo a uno menos
    WHILE @Mes < @Plazo  -- Incluir hasta un mes menos de la fecha de término
    BEGIN
        -- Calcular el mes y el año correspondiente
        DECLARE @MesActual INT = MONTH(DATEADD(MONTH, @Mes, @FechaInicio));
        DECLARE @AnioActual INT = YEAR(DATEADD(MONTH, @Mes, @FechaInicio));

        -- Insertar o actualizar las horas planeadas en la tabla temporal
        MERGE INTO #HorasPorMes AS target
        USING (SELECT 
            @MesActual AS Mes, 
            @AnioActual AS Anio,
            (SELECT ISNULL(SUM(HU.HH_SOCIOS), 0) / CAST(@Plazo AS DECIMAL(18, 2)) FROM HH_USUARIO_HISTORIAL HU WHERE HU.ID_PROYECTO = @ProyectoId) AS HorasSociosPlaneadas,
            (SELECT ISNULL(SUM(HU.HH_STAFF), 0) / CAST(@Plazo AS DECIMAL(18, 2)) FROM HH_USUARIO_HISTORIAL HU WHERE HU.ID_PROYECTO = @ProyectoId) AS HorasStaffPlaneadas,
            (SELECT ISNULL(SUM(HU.HH_CONSULTORA), 0) / CAST(@Plazo AS DECIMAL(18, 2)) FROM HH_USUARIO_HISTORIAL HU WHERE HU.ID_PROYECTO = @ProyectoId) AS HorasConsultorAPlaneadas,
            (SELECT ISNULL(SUM(HU.HH_CONSULTORB), 0) / CAST(@Plazo AS DECIMAL(18, 2)) FROM HH_USUARIO_HISTORIAL HU WHERE HU.ID_PROYECTO = @ProyectoId) AS HorasConsultorBPlaneadas,
            (SELECT ISNULL(SUM(HU.HH_CONSULTORC), 0) / CAST(@Plazo AS DECIMAL(18, 2)) FROM HH_USUARIO_HISTORIAL HU WHERE HU.ID_PROYECTO = @ProyectoId) AS HorasConsultorCPlaneadas) AS source
        ON target.Mes = source.Mes AND target.Anio = source.Anio
        WHEN MATCHED THEN
            UPDATE SET 
                target.HorasSociosPlaneadas = ISNULL(target.HorasSociosPlaneadas, 0) + source.HorasSociosPlaneadas,
                target.HorasStaffPlaneadas = ISNULL(target.HorasStaffPlaneadas, 0) + source.HorasStaffPlaneadas,
                target.HorasConsultorAPlaneadas = ISNULL(target.HorasConsultorAPlaneadas, 0) + source.HorasConsultorAPlaneadas,
                target.HorasConsultorBPlaneadas = ISNULL(target.HorasConsultorBPlaneadas, 0) + source.HorasConsultorBPlaneadas,
                target.HorasConsultorCPlaneadas = ISNULL(target.HorasConsultorCPlaneadas, 0) + source.HorasConsultorCPlaneadas
        WHEN NOT MATCHED THEN
            INSERT (Mes, Anio, HorasSociosPlaneadas, HorasStaffPlaneadas, HorasConsultorAPlaneadas, HorasConsultorBPlaneadas, HorasConsultorCPlaneadas)
            VALUES (source.Mes, source.Anio, source.HorasSociosPlaneadas, source.HorasStaffPlaneadas, source.HorasConsultorAPlaneadas, source.HorasConsultorBPlaneadas, source.HorasConsultorCPlaneadas);

        SET @Mes = @Mes + 1;
    END;

    -- Insertar 0 horas planeadas para el último mes
    SET @Mes = @Plazo;  -- El último mes en la lógica es igual al plazo
    SET @MesActual = MONTH(DATEADD(MONTH, @Mes, @FechaInicio));
    SET @AnioActual = YEAR(DATEADD(MONTH, @Mes, @FechaInicio));

    MERGE INTO #HorasPorMes AS target
    USING (SELECT @MesActual AS Mes, @AnioActual AS Anio, 0 AS HorasSociosPlaneadas, 0 AS HorasStaffPlaneadas, 0 AS HorasConsultorAPlaneadas, 0 AS HorasConsultorBPlaneadas, 0 AS HorasConsultorCPlaneadas) AS source
    ON target.Mes = source.Mes AND target.Anio = source.Anio
    WHEN MATCHED THEN
        UPDATE SET 
            target.HorasSociosPlaneadas = ISNULL(target.HorasSociosPlaneadas, 0) + source.HorasSociosPlaneadas,
            target.HorasStaffPlaneadas = ISNULL(target.HorasStaffPlaneadas, 0) + source.HorasStaffPlaneadas,
            target.HorasConsultorAPlaneadas = ISNULL(target.HorasConsultorAPlaneadas, 0) + source.HorasConsultorAPlaneadas,
            target.HorasConsultorBPlaneadas = ISNULL(target.HorasConsultorBPlaneadas, 0) + source.HorasConsultorBPlaneadas,
            target.HorasConsultorCPlaneadas = ISNULL(target.HorasConsultorCPlaneadas, 0) + source.HorasConsultorCPlaneadas
    WHEN NOT MATCHED THEN
        INSERT (Mes, Anio, HorasSociosPlaneadas, HorasStaffPlaneadas, HorasConsultorAPlaneadas, HorasConsultorBPlaneadas, HorasConsultorCPlaneadas)
        VALUES (source.Mes, source.Anio, source.HorasSociosPlaneadas, source.HorasStaffPlaneadas, source.HorasConsultorAPlaneadas, source.HorasConsultorBPlaneadas, source.HorasConsultorCPlaneadas);

    -- Seleccionar los resultados finales
    SELECT 
        Mes, 
        Anio,
        SUM(HorasSociosEjecutadas) AS HorasSociosEjecutadas,
        SUM(HorasStaffEjecutadas) AS HorasStaffEjecutadas,
        SUM(HorasConsultorAEjecutadas) AS HorasConsultorAEjecutadas,
        SUM(HorasConsultorBEjecutadas) AS HorasConsultorBEjecutadas,
        SUM(HorasConsultorCEjecutadas) AS HorasConsultorCEjecutadas,
        SUM(HorasSociosPlaneadas) AS HorasSociosPlaneadas,
        SUM(HorasStaffPlaneadas) AS HorasStaffPlaneadas,
        SUM(HorasConsultorAPlaneadas) AS HorasConsultorAPlaneadas,
        SUM(HorasConsultorBPlaneadas) AS HorasConsultorBPlaneadas,
        SUM(HorasConsultorCPlaneadas) AS HorasConsultorCPlaneadas
    FROM 
        #HorasPorMes
    GROUP BY 
        Mes, Anio
    ORDER BY 
        Anio, Mes;

    -- Limpiar la tabla temporal
    DROP TABLE #HorasPorMes;
END




ALTER PROCEDURE [dbo].[EDITARUSUARIO]
    @IDUSUARIO INT,
    @NOMBRE VARCHAR(200),
    @NOMBRE_USUARIO VARCHAR(200),
    @TELEFONO VARCHAR(20),
    @EMAIL VARCHAR(90),
    @NUMERO_HORAS_SEMANALES INT,
    @COSTO_UNITARIO DECIMAL(10,2),
    @PORCENTAJEHORAS FLOAT,
    @FECHAINICIO DATE,
    @FECHAFIN DATE
AS
BEGIN
    DECLARE @IDRECURSO INT;
    DECLARE @HHANUALES DECIMAL(10,2);
    DECLARE @TOTALHHASIGNADAS INT;
    DECLARE @RECURSO VARCHAR(100);
    DECLARE @PROMEDIO_COSTO DECIMAL(10,2);
    DECLARE @TOTALHHANUAL DECIMAL(10,2);
    DECLARE @HHANUALES_ACTUAL DECIMAL(10,2);

    -- Cálculo de horas anuales
    SET @HHANUALES = dbo.CALCULARHHANUALES(@NUMERO_HORAS_SEMANALES, @PORCENTAJEHORAS);

    -- Obtener tipo de recurso (Socio, Staff, Consultor Externo) para este usuario
    SELECT @RECURSO = R.NOMBRE_RECURSO, @HHANUALES_ACTUAL = R.HH_ANUALES
    FROM USUARIO U
    INNER JOIN RECURSO R ON R.ID = U.ID_RECURSO
    WHERE U.ID = @IDUSUARIO;

   
    -- Actualizar el recurso con los nuevos datos
    UPDATE RECURSO 
    SET 
        NUMERO_HORAS = @NUMERO_HORAS_SEMANALES,
        COSTO_UNITARIO = @COSTO_UNITARIO,
        PROCENTAJE_PROYECTO = @PORCENTAJEHORAS,
        DESDE = @FECHAINICIO,
        HASTA = @FECHAFIN,
        HH_ANUALES = @HHANUALES
    WHERE ID = (SELECT ID_RECURSO FROM USUARIO WHERE ID = @IDUSUARIO);

    -- Actualizar el usuario con los nuevos datos
    UPDATE USUARIO 
    SET 
        NOMBRE = @NOMBRE,
        NOMBRE_USUARIO = @NOMBRE_USUARIO,
        TELEFONO = @TELEFONO,
        EMAIL = @EMAIL 
    WHERE ID = @IDUSUARIO;

    -- Actualizar el total de horas anuales
    IF (@RECURSO = 'Socio' OR @RECURSO = 'Staff')
    BEGIN
        -- Eliminar el HHANUALES actual del total
        SELECT @TOTALHHANUAL = TOTAL_HH_ANUALES FROM TOTAL_RECURSOS WHERE TIPO_RECURSO = @RECURSO;
        
        -- Actualizar el total
        SET @TOTALHHANUAL = @TOTALHHANUAL - @HHANUALES_ACTUAL + @HHANUALES;

        -- Actualizar en TOTAL_RECURSOS
        UPDATE TOTAL_RECURSOS
        SET TOTAL_HH_ANUALES = @TOTALHHANUAL
        WHERE TIPO_RECURSO = @RECURSO;

        -- Actualizar el costo promedio si es necesario
        SELECT @PROMEDIO_COSTO = AVG(COSTO_UNITARIO)
        FROM RECURSO
        WHERE NOMBRE_RECURSO = @RECURSO;

        UPDATE COSTO_PROMEDIO
        SET VALOR = @PROMEDIO_COSTO
        WHERE TIPO_RECURSO = @RECURSO;
    END;

END;