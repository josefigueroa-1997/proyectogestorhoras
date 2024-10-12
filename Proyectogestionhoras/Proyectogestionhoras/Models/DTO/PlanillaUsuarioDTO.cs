namespace Proyectogestionhoras.Models.DTO
{
    public class PlanillaUsuarioDTO
    {
        public DateTime FechaRegistro { get; set; }
        public string? NombreProyecto { get; set; }
        public string? NumProyecto { get; set; }
        public int IDPROYECTO { get; set; }
        public string? NombreActividad { get; set; }
        public int HHregistradas { get; set; }
        public string? Observaciones { get; set; }


    }
}
