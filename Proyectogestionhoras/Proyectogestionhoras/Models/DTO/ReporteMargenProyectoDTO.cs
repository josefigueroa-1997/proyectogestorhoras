namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteMargenProyectoDTO
    {
        public string? NumProyecto { get;set; }
        public string? NombreProyecto { get; set; } 
        public decimal Monto { get; set; }
        public string? Unegocios { get; set; }
        public string? ccosto { get; set; }
        public string? tipologia { get; set; }
        public decimal totalcostospresupuesto { get; set; }
        public decimal totalingresorealesforecast { get; set; }
        public decimal totalcostorealforecast { get; set; }


    }
}
