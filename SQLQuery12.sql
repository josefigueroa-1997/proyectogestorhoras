ALTER PROCEDURE GENERARFACTURAPROYECTO
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
	DECLARE @SEGMENTO VARCHAR(200);
	DECLARE @CUENTA VARCHAR(100);
	DECLARE @IDCUENTA INT;
    -- Obtener la información del proyecto usando @IDPROYECTO
    SELECT @PLAZO = P.PLAZO,
           @FECHAINICIO = P.FECHA_INICIO,
           @MONTO = PE.MONTO,
           @AFECTAIVA = PE.AFECTAIVA,
           @VALORIVA = 0.19,
		   @SEGMENTO =  S.NOMBRE,
		   @IDCUENTA = c.IDCUENTA,
		   @CUENTA = c.CUENTA
    FROM PROYECTO P
    INNER JOIN PRESUPUESTO PE ON PE.ID = P.ID_PRESUPUESTO
	INNER JOIN FACTURA F ON F.ID_PROYECTO = P.ID
	INNER JOIN SEGMENTO S ON F.IDSEGMENTO = S.ID
	INNER JOIN CUENTA C ON C.ID = S.ID_CUENTA
    WHERE P.ID = @IDPROYECTO; -- Filtro para el proyecto específico

    -- Calcular el monto mensual
    SET @MONTO_MENSUAL = @MONTO / @PLAZO;

    -- Crear una tabla temporal para almacenar los resultados
    CREATE TABLE #FacturasTemp (
		IdCuenta int,
		Cuenta varchar(100),
		Segmento VARCHAR(200),
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
        INSERT INTO #FacturasTemp (IdCuenta,Cuenta,Segmento,FechaFactura, Monto, MontoIVA, Total)
        VALUES (@IDCUENTA,@CUENTA,@SEGMENTO,@FECHAFACTURA, @MONTO_MENSUAL, @MONTO_CON_IVA - @MONTO_MENSUAL, @MONTO_CON_IVA);

        -- Incrementar el mes
        SET @MES = @MES + 1;
    END

    -- Mostrar las facturas generadas
    SELECT IdCuenta AS 'IdCuenta',
		   Cuenta AS 'Cuenta',
		   Segmento AS 'Segmento',
		   FechaFactura AS 'Fecha_Factura', 
           Monto AS 'Neto', 
           MontoIVA AS 'IVA', 
           Total AS 'Total'
    FROM #FacturasTemp;

    -- Eliminar la tabla temporal
    DROP TABLE #FacturasTemp;

    -- Restablecer el comportamiento normal si es necesario
    SET NOCOUNT OFF;
END




ALTER PROCEDURE [dbo].[OBTENERPROYECTOS]
    @ID INT,
    @IDCLIENTE INT,
    @NOMBRE VARCHAR(200),
    @ID_TIPOEMPRESA INT,
    @STATUS_PROYECTO INT,
    @NUMPROYECTO VARCHAR(100),
    @IDTIPOLOGIA INT,
    @UNIDADNEGOCIO INT,
    @IDCENTROCOSTO INT
AS
BEGIN
    SELECT 
        P.ID,
        P.NUM_PROYECTO,
        Un.TIPO_UNEGOCIO,
		un.ID as IDUNEGOCIO,
		costo.ID AS IDCOSTO,
        COSTO.TIPO_CCOSTO,
        CU.CODIGO,
		C.ID as IDCLIENTE,
        C.NOMBRE AS NOMBRECLIENTE,
        P.NOMBRE AS NOMBREPROYECTO,
        T.TIPO_TIPOLOGIA,
		t.ID AS IDTIPOLOGIA,
        E.TIPO_EMPRESA,
		e.ID AS IDEMPRESA,
        PR.AFECTAIVA,
		PR.MONTO,
		PR.MONEDA,
        ST.TIPO_STATUS,
		st.ID AS STATUSPROYECTO,
        P.PROBABILIDAD,
        P.PORCENTAJE_PROBABILIDAD,
        P.PLAZO,
        P.FECHA_INICIO,
        P.FECHA_TERMINO,
        P.FECHA_PLAZO_NEG,
		S.ID AS IDDEPARTAMENTO,
        S.NOMBRE AS NOMBREDEPARTAMENTO,

        -- Horas para Socios
        MAX(ISNULL(CASE 
            WHEN R.NOMBRE_RECURSO = 'Socio' THEN UP.HH_SOCIOS 
            END,0)) AS HHSOCIOS,

        -- Cuenta para Socios
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Socio' THEN CT.CUENTA
            END) AS CUENTA_SOCIOS,

        -- ID de Cuenta para Socios
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Socio' THEN CT.IDCUENTA
            END) AS IDCUENTA_SOCIOS,

			--SEGMENTO SOCIOS--
			MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Socio' THEN SE.NOMBRE
            END) AS SEGMENTO_SOCIOS,

		--COSTO UNITARIO SOCIOS
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Socio' THEN UP.CostoUnitarioAsignado
            END) AS COSTO_SOCIO,

        -- Horas para Staff
        MAX(ISNULL(CASE 
            WHEN R.NOMBRE_RECURSO = 'Staff' THEN UP.HH_STAFF 
            END,0)) AS HHSTAFF,

        -- Cuenta para Staff
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Staff' THEN CT.CUENTA
            END) AS CUENTA_STAFF,

        -- ID de Cuenta para Staff
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Staff' THEN CT.IDCUENTA
            END) AS IDCUENTA_STAFF,


		--SEGMENTO STAFF--
			MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Staff' THEN SE.NOMBRE
            END) AS SEGMENTO_STAFF,

		--COSTO UNITARIO STAFF
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Staff' THEN UP.CostoUnitarioAsignado
            END) AS COSTO_STAFF,

        -- Horas para Consultor A
        MAX(ISNULL(CASE 
				WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'A' 
				THEN UP.HH_CONSULTORA 
				END, 0)) AS HH_CONSULTOR_A,

        -- Cuenta para Consultor A
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'A' 
            THEN CT.CUENTA
            END) AS CUENTA_CONSULTOR_A,

        -- ID de Cuenta para Consultor A
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'A' 
            THEN CT.IDCUENTA
            END) AS IDCUENTA_CONSULTOR_A,
		--SEGMENTO CONSULTOR A--
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'A' 
            THEN SE.NOMBRE
            END) AS SEGMENTO_CONSULTOR_A,

		--COSTO UNITARIO CONSULTOR A
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'A' 
			THEN UP.COSTOCONSULTORA
            END) AS COSTO_CONSULTORA,


        -- Horas para Consultor B
        MAX(ISNULL(CASE 
				WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'B' 
				THEN UP.HH_CONSULTORA 
				END, 0)) AS HH_CONSULTOR_B,

        -- Cuenta para Consultor B
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'B' 
            THEN CT.CUENTA
            END) AS CUENTA_CONSULTOR_B,

        -- ID de Cuenta para Consultor B
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'B' 
            THEN CT.IDCUENTA
            END) AS IDCUENTA_CONSULTOR_B,
		--SEGMENTO CONSULTOR B--
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'B' 
            THEN SE.NOMBRE
            END) AS SEGMENTO_CONSULTOR_B,

		--COSTO UNITARIO CONSULTOR B
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'B' 
			THEN UP.COSTOCONSULTORB
            END) AS COSTO_CONSULTORB,

        -- Horas para Consultor C
        MAX(ISNULL(CASE 
				WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'C' 
				THEN UP.HH_CONSULTORA 
				END, 0)) AS HH_CONSULTOR_C,

        -- Cuenta para Consultor C
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'C' 
            THEN CT.CUENTA
            END) AS CUENTA_CONSULTOR_C,

        -- ID de Cuenta para Consultor C
        MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'C' 
            THEN CT.IDCUENTA
            END) AS IDCUENTA_CONSULTOR_C,
		--SEGMENTO CONSULTOR A--
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'C' 
            THEN SE.NOMBRE
            END) AS SEGMENTO_CONSULTOR_C,
		--COSTO UNITARIO CONSULTOR C
		MAX(CASE 
            WHEN R.NOMBRE_RECURSO = 'Consultor Externo' AND CHAR(65 + RN) = 'C' 
			THEN UP.COSTOCONSULTORC
            END) AS COSTO_CONSULTORC
    FROM PROYECTO P
    INNER JOIN SUCURSAL_CLIENTE SC ON SC.ID = P.ID_CLIENTE_SUCURSAL
    INNER JOIN USUARIO_PROYECTO UP ON UP.ID_PROYECTO = P.ID
    INNER JOIN USUARIO U ON U.ID = UP.ID_USUARIO
    INNER JOIN RECURSO R ON R.ID = U.ID_RECURSO
    INNER JOIN CLIENTE C ON C.ID = SC.ID_CLIENTE
    INNER JOIN SUCURSAL S ON S.ID = SC.ID_SUCURSAL
    INNER JOIN TIPOLOGIA T ON T.ID = P.ID_TIPOLOGIA
    INNER JOIN EMPRESA E ON E.ID = P.TIPO_EMPRESA
    INNER JOIN CCOSTO_UNEGOCIO CU ON CU.ID = P.ID_CCOSTO_UNEGOCIO 
    INNER JOIN CCOSTO COSTO ON COSTO.ID = CU.ID_CCOSTO
    INNER JOIN UNEGOCIO Un ON Un.ID = CU.ID_UNEGOCIO
    INNER JOIN PRESUPUESTO PR ON PR.ID = P.ID_PRESUPUESTO
    INNER JOIN STATUS_PROYECTO ST ON ST.ID = P.STATUS_PROYECTO
	INNER JOIN SEGMENTO SE ON UP.IDSEGMENTO = SE.ID
	INNER JOIN CUENTA CT ON CT.ID = SE.ID_CUENTA 
    CROSS APPLY (
         SELECT 
        ROW_NUMBER() OVER (
            PARTITION BY R.NOMBRE_RECURSO, UP.ID_PROYECTO 
            ORDER BY UP.ID_PROYECTO, U.ID
        ) - 1 AS RN
    FROM USUARIO_PROYECTO UP2
    INNER JOIN USUARIO U2 ON UP2.ID_USUARIO = U2.ID
    INNER JOIN RECURSO R2 ON U2.ID_RECURSO = R2.ID
    WHERE R2.NOMBRE_RECURSO = R.NOMBRE_RECURSO
      AND UP2.ID_PROYECTO = UP.ID_PROYECTO
    ) AS CA
    WHERE (P.ID = @ID OR @ID = 0 OR @ID IS NULL) 
        AND (C.ID = @IDCLIENTE OR @IDCLIENTE = 0 OR @IDCLIENTE IS NULL) 
        AND (P.NOMBRE = @NOMBRE OR @NOMBRE IS NULL)
        AND (P.TIPO_EMPRESA = @ID_TIPOEMPRESA OR @ID_TIPOEMPRESA = 0 OR @ID_TIPOEMPRESA IS NULL) 
        AND (ST.ID = @STATUS_PROYECTO OR @STATUS_PROYECTO = 0 OR @STATUS_PROYECTO IS NULL)
        AND (P.NUM_PROYECTO = @NUMPROYECTO OR @NUMPROYECTO IS NULL)
        AND (P.ID_TIPOLOGIA = @IDTIPOLOGIA OR @IDTIPOLOGIA = 0 OR @IDTIPOLOGIA IS NULL)
        AND (U.ID = @UNIDADNEGOCIO OR @UNIDADNEGOCIO = 0 OR @UNIDADNEGOCIO IS NULL)
        AND (COSTO.ID = @IDCENTROCOSTO OR @IDCENTROCOSTO = 0 OR @IDCENTROCOSTO IS NULL)
    GROUP BY 
        P.ID, P.NUM_PROYECTO, Un.TIPO_UNEGOCIO, PR.MONTO,PR.MONEDA,COSTO.TIPO_CCOSTO, 
        CU.CODIGO, C.NOMBRE, P.NOMBRE, T.TIPO_TIPOLOGIA, 
        E.TIPO_EMPRESA, PR.AFECTAIVA, ST.TIPO_STATUS, 
        P.PROBABILIDAD, P.PORCENTAJE_PROBABILIDAD, P.PLAZO, 
        P.FECHA_INICIO, P.FECHA_TERMINO, P.FECHA_PLAZO_NEG,un.ID,costo.ID, 
        t.ID,e.ID,st.ID,C.ID,S.ID,S.NOMBRE;
END;

