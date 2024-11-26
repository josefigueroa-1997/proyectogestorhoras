namespace Proyectogestionhoras.Models.DTO
{
    public class EgresosServiciosGastosDTO
    {
        public string? NumProyecto { get; set; }
        public string? Nombre { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal? MontosServiciosProyectado { get; set; }
        public decimal? MontoSerivicioReal { get; set; }
        public decimal? MontosGastosProyectado { get; set; }
        public decimal? MontoGastoReal { get; set; }
        public string? ServicioNombre { get; set; }
        public string? GastoNombre { get; set; }
        public int idcuentaservicio { get; set; }
        public string? CuentaServicio { get; set; }
        public int idcuentagasto { get; set; }
        public string? Cuentagasto { get; set; }
    }
}
