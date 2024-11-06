namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteBalanceDTO
    {
        public string? nombreproyecto { get; set; }
        public string? numeroproyecto { get; set; }
        public string? tipologia { get; set; }
        public string? nombrecliente { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public string? tiporecurso { get; set; }
        public decimal totalhorasporrecurso { get; set; }
    }
}
