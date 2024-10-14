namespace Proyectogestionhoras.Models.DTO
{
    public class DatosGanttDTO
    {

        public string? NombreProyecto { get; set; }
        public string? NombreUsuario { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public int TotalHoras { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public List<HorasPorMes> HorasPorMes { get; set; } = new List<HorasPorMes>();
    }
}
