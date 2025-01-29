namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteMetaFacturacionesDTO
    {
        public string? Tipo { get; set; }
        public string? Nombre { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
    }
}
