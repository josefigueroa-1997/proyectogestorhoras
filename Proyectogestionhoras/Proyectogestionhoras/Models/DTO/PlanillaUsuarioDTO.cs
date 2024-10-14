namespace Proyectogestionhoras.Models.DTO
{
    public class PlanillaUsuarioDTO
    {
        public int IdPlanilla { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? NombreProyecto { get; set; }
        public string? NumProyecto { get; set; }
        public int IDPROYECTO { get; set; }
        public string? NombreActividad { get; set; }
        public int HHregistradas { get; set; }
        public string? Observaciones { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int IDCUENTA { get; set; }
        public string? Cuenta { get; set; }
        public string? NombreSegmento { get; set;}

    }
}
