namespace Proyectogestionhoras.Models.DTO
{
    public class EgresosExcelDTO
    {
        public string? numproyecto { get; set; }
        public string? egreso { get; set; }
        public string? proveedor { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string? Estado { get; set; }
        public string? EstadoVenta { get; set; }
        public string? Glosa { get; set; }
        public string? Tipo { get; set; }

    }
}
