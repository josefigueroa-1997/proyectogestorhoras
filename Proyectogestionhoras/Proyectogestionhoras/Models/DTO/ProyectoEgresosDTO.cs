namespace Proyectogestionhoras.Models.DTO
{
    public class ProyectoEgresosDTO
    {
        public int IdProyecto { get; set; }
        public string? NombreProyecto { get; set; }
        public List<EgresosExcelDTO>? Egresos { get; set; }

    }
}
