namespace Proyectogestionhoras.Models.DTO
{
    public class IngresosExcelDTO
    {
        public string? numproyecto { get; set; }
        public string? numdocumento { get; set; }
        public DateTime fechaemision { get; set; }
        public DateTime Fechapago { get; set; }
        public decimal? Monto { get; set; }
        public decimal? iva { get; set; }
        public string? Estado { get; set; }
        public string? EstadoVenta { get; set; }
        public string? Glosa { get; set; }
        
    }
}
