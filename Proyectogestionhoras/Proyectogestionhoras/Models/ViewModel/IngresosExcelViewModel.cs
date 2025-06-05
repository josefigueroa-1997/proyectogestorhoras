namespace Proyectogestionhoras.Models.ViewModel
{
    public class IngresosExcelViewModel
    {
        public int? Idproyecto { get; set; }
        public string? numdocumento { get; set; }
        public decimal? Monto { get; set; }
        public decimal? iva { get; set; }
        public DateTime? Fechapago { get; set; }
        public DateTime? Fechaemision { get; set; }
        public string? Observacion { get; set; }
        public string? Estado { get; set; }
        public string? Venta { get; set; }
        
    }
}
