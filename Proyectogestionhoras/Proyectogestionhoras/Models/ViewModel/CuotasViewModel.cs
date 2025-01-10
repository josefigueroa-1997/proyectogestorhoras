namespace Proyectogestionhoras.Models.ViewModel
{
    public class CuotasViewModel
    {
        public int IdCuota { get; set; }
        public DateTime FechaEmision { get;set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoCuota {  get; set; }
        public string? Observacion { get; set; }
    }
}
