namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteMargenProyectoDTO
    {
        public string? NumProyecto { get;set; }
        public string? NombreProyecto { get; set; } 
        public decimal Monto { get; set; }
        public decimal TotalGastos { get; set; }    
        public decimal TotalServicios { get; set; }
        public decimal CostosSocios { get; set; }
        public decimal CostosStaff { get; set; }
        public decimal CostoConsultorA { get; set; }
        public decimal CostoConsultorB { get; set; }
        public decimal CostoConsultorC { get; set; }
        public decimal Margen { get; set; }
        public string? PorcentajeMargen { get; set; }
    }
}
