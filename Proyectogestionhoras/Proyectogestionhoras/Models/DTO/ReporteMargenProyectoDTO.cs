namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteMargenProyectoDTO
    {
        public string? NumProyecto { get;set; }
        public string? NombreProyecto { get; set; } 
        public decimal Monto { get; set; }
        public decimal MontoReal { get; set; }
        public decimal MontoForecast { get; set; }
        public decimal TotalGastos { get; set; }    
        public decimal TotalGastosReales { get; set; }    
        public decimal TotalGastosForecast { get; set; }    
        public decimal TotalServicios { get; set; }
        public decimal TotalServiciosReales { get; set; }
        public decimal TotalServiciosForecast { get; set; }
        public decimal CostosSocios { get; set; }
        public decimal CostosSociosReales { get; set; }
        public decimal CostosSociosForecast { get; set; }
        public decimal CostosStaff { get; set; }
        public decimal CostosStaffReales { get; set; }
        public decimal CostosStaffForecast { get; set; }
        public decimal CostoConsultorA { get; set; }
        public decimal CostoConsultorReal { get; set; }
        public decimal CostoConsultorForecast { get; set; }
        public decimal Margen { get; set; }
        public string? PorcentajeMargen { get; set; }
    }
}
