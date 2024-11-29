namespace Proyectogestionhoras.Models.ViewModel
{
    public class GastosHHViewModel
    {
        public int IdGastoHH { get; set; }
        public string? Tiporecurso { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public DateTime? Fechapago { get; set; }
        public decimal? Monto { get; set; }
        public decimal? HHtotales { get; set; }
        public string? Observacion { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Reajuste { get; set; }
    }
}
