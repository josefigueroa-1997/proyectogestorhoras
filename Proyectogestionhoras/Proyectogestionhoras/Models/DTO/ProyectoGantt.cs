namespace Proyectogestionhoras.Models.DTO
{
    public class ProyectoGantt
    {
        public string? NombreProyecto { get; set; }
        public DateTime FechaInicio { get; set; }  // Agregar propiedad
        public DateTime FechaTermino { get; set; }
        public List<HorasPorMes>? HorasPorMes { get; set; }
    }
}
