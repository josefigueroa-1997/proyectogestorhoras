namespace Proyectogestionhoras.Models.DTO
{
    public class FacturaProyectoDTO
    {
        public int IdCuenta { get; set; }
        public string? Cuenta { get; set; }
        public string? Segmento { get; set; }
        public DateTime? Fecha_Factura { get; set; }
        public decimal? Neto { get; set; }
        public decimal? IVA { get; set; }
        public decimal? Total { get; set; }
    }
}
