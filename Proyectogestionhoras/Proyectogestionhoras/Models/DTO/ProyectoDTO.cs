namespace Proyectogestionhoras.Models.DTO
{
    public class ProyectoDTO
    {
        public int Id { get; set; }
        public string? numproyecto { get; set; }
        public string? Tipo_Unegocio { get; set; }
        public int IDUNEGOCIO { get; set; }
        public int IDCOSTO { get; set; }
        public string? Tipo_CCosto { get; set; }
        public string? Codigo { get; set; }
        public int IDCLIENTE { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; }
        public string? Tipo_Tipologia { get; set; }
        public int IDTIPOLOGIA { get; set; }
        public string? Tipo_Empresa { get; set; }
        public int IDEMPRESA { get; set; }
        public int IDPRESUPESTO { get; set; }
        public string? AfectaIva { get; set; }
        public string? Tipo_Status { get; set; }
        public int STATUSPROYECTO { get; set; }
        public string? Probabilidad { get; set; }
        public decimal? Porcentaje_Probabilidad { get; set; }
        public int Plazo { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Termino { get; set;}
        public DateTime? Fecha_Plazo_Neg { get; set;}

        public string? NOMBREDEPARTAMENTO { get; set; }
        public int IDDEPARTAMENTO { get; set; }
        public decimal? MONTO { get; set; }
        public string? MONEDA { get; set; }
        

        public int HHSOCIOS { get; set; }
        public string? CUENTA_SOCIOS { get; set; }
        public int IDCUENTA_SOCIOS { get; set; }
        public string? SEGMENTO_SOCIOS { get; set; }
        public decimal? COSTO_SOCIO { get; set; }

        public int HHSTAFF { get; set; }
        public string? CUENTA_STAFF { get; set; }
        public int IDCUENTA_STAFF { get; set; }
        public string? SEGMENTO_STAFF { get; set; }
        public decimal? COSTO_STAFF { get; set; }
        
        public int HH_CONSULTOR_A { get; set; }
        public string? CUENTA_CONSULTOR_A { get; set; }
        public int IDCUENTA_CONSULTOR_A { get; set; }
        public string? SEGMENTO_CONSULTOR_A { get; set; }
        public decimal? COSTO_CONSULTORA { get; set; }

        public int HH_CONSULTOR_B { get; set; }
        public string? CUENTA_CONSULTOR_B { get; set; }
        public int IDCUENTA_CONSULTOR_B { get; set; }
        public string? SEGMENTO_CONSULTOR_B { get; set; }
        public decimal? COSTO_CONSULTORB { get; set; }

        public int HH_CONSULTOR_C { get; set; }
        public string? CUENTA_CONSULTOR_C { get; set; }
        public int IDCUENTA_CONSULTOR_C { get; set; }
        public string? SEGMENTO_CONSULTOR_C { get; set; }
        public decimal? COSTO_CONSULTORC { get; set; }


        public string? NOMBRE_RECURSO { get; set; }
        public string? TIPO_CONSULTOR { get; set; }

        public decimal MontoPresupuesto { get; set; }
        public decimal CostoSocioPresupuesto { get; set; }
        public decimal CostoStaffPresupuesto { get; set; }
        public decimal CostoConsultorAPresupuesto { get; set; }
        public decimal CostoConsultorBPresupuesto { get; set; }
        public decimal CostoConsultorCPresupuesto { get; set; }
        public decimal MontoOrigenExtranjero { get; set; }
        public decimal TasaCambio { get; set; }
        public int idpresupuesto { get; set; }
        public int CantidadCuotas { get; set; }
    }
}
