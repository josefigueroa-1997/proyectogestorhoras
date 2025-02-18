namespace Proyectogestionhoras.Models.DTO
{
    public class PlanillaUsuarioDTO
    {
        public int IdPlanilla { get; set; }
        public int IdRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? NombreProyecto { get; set; }
        public string? Nombre { get; set; }
        public string? NumProyecto { get; set; }
        public int IDPROYECTO { get; set; }
        public int IdActividad { get; set; }
        public int estado { get; set; }
        public int idusuproy { get; set; }
        public string? NombreActividad { get; set; }
        public decimal HHregistradas { get; set; }
        public string? Observaciones { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int IDCUENTA { get; set; }
        public string? Cuenta { get; set; }
        public string? NombreSegmento { get; set;}
        public string? NombreUsuario { get; set;}
        public string? Rol { get; set;}
        public decimal CostoUnitario  { get; set;}
        public decimal CostoTotal  { get; set;}

    }
}
