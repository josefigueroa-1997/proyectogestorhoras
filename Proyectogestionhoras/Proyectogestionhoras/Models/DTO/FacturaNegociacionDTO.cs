namespace Proyectogestionhoras.Models.DTO
{
    public class FacturaNegociacionDTO
    {
        public DateTime FechaFactura { get; set; }
        public decimal Neto { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}
