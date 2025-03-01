USE [PROYECTO_CONTROL_HORAS]
GO
/****** Object:  StoredProcedure [dbo].[REPORTEFACTURAS]    Script Date: 19/10/2024 18:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[REPORTEFACTURAS]
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
        Monto DECIMAL(18, 2),
        MontoIVA DECIMAL(18, 2),
        Total DECIMAL(18, 2)
    );

    -- Obtener los proyectos y sus detalles necesarios para las facturas
    INSERT INTO #FacturasTemp (Num_Proyecto, Nombre, Estado, Tipologia, NombreCliente, FechaFactura, Monto, MontoIVA, Total)
    SELECT 
        P.NUM_PROYECTO,
        P.NOMBRE,
        SP.TIPO_STATUS,
        T.TIPO_TIPOLOGIA,
        C.NOMBRE,
        DATEADD(MONTH, m.MonthNumber + 1, P.FECHA_INICIO) AS FechaFactura, -- Ajustar para que empiece un mes después
        ROUND(PE.MONTO / P.PLAZO, 2) AS Monto,
        CASE 
            WHEN PE.AFECTAIVA = 'SI' THEN ROUND(ROUND(PE.MONTO / P.PLAZO, 2) * 0.19, 2)
            ELSE 0
        END AS MontoIVA,
        ROUND(
            ROUND(PE.MONTO / P.PLAZO, 2) + 
            CASE 
                WHEN PE.AFECTAIVA = 'SI' THEN ROUND(ROUND(PE.MONTO / P.PLAZO, 2) * 0.19, 2)
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
    CROSS JOIN (
        SELECT TOP 12 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS MonthNumber
        FROM master..spt_values
    ) m
    WHERE m.MonthNumber < P.PLAZO;

    -- Mostrar las facturas generadas
    SELECT 
        Num_Proyecto AS 'Num_Proyecto',
        Nombre AS 'Nombre',
        Estado AS 'TIPO_STATUS',
        Tipologia AS 'TIPO_TIPOLOGIA',
        NombreCliente AS 'NOMBRECLIENTE',
        FechaFactura AS 'Fecha_Factura', 
        ROUND(Monto, 2) AS 'Neto',
        ROUND(MontoIVA, 2) AS 'IVA',
        ROUND(Total, 2) AS 'Total'
    FROM 
        #FacturasTemp;

    DROP TABLE #FacturasTemp;

    SET NOCOUNT OFF;
END;
