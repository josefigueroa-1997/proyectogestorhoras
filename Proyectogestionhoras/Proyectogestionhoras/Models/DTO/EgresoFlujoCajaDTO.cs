namespace Proyectogestionhoras.Models.DTO
{
    public class EgresoFlujoCajaDTO
    {
        public string? NumProyecto { get; set; }
        public string? Nombre { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal? MontosSocioProyectado { get; set; }
        public decimal? MontoSocioReal { get; set; }
        public decimal? MontosStaffProyectado { get; set; }
        public decimal? MontoStaffReal { get; set; }
        public decimal? MontosConsultoresProyectado { get; set; }
        public decimal? MontoConsultoresReal { get; set; }
        public decimal? MontosServiciosProyectado { get; set; }
        public decimal? MontoSerivicioReal { get; set; }
        public decimal? MontosGastosProyectado { get; set; }
        public decimal? MontoGastoReal { get; set; }
    }
}
