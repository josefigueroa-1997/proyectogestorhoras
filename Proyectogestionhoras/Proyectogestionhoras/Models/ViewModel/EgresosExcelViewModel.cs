namespace Proyectogestionhoras.Models.ViewModel
{
    public class EgresosExcelViewModel
    {
        public int Idegreso { get; set; }
        public int Idpeoyecto { get; set; }
        public int? Idproveedor { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Observacion { get; set; }
        public string? Estado { get; set; }
        public string? Venta { get; set; }
        public string? Tiposervicio { get; set; }
    }
}
