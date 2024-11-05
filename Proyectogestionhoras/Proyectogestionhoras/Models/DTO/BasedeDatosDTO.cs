namespace Proyectogestionhoras.Models.DTO
{
    public class BasedeDatosDTO
    {
        public int Id { get; set; }
        public string? NumProyecto { get; set; }
        public string? Unegocio { get; set; }
        public string? CCosto { get; set; }
        public string? Codigo { get; set; }
        public string? Cliente { get; set; }
        public string? NombreProyecto { get; set; }
        public string? Tipologia { get; set; }
        public string? Empresa { get; set; }
        public string? AfectaIva { get; set; }
        public decimal Monto { get; set; }
        public string? Moneda { get; set; }
        public string? Status { get; set; }
        public string? Probabilidad { get; set; }
        public decimal Porcentaje { get; set; }
        public int Plazo { get; set; }
        public DateTime? Fechainicio { get; set; }
        public DateTime? Fechatermino { get; set; }
        public DateTime? FechaPlazo { get; set; }
        public string? Departamento { get; set; }
        public int HHsocios { get; set; }
        public int HHstaff { get; set; }
        public int HHConsultorA { get; set; }
        public int HHConsultorB { get; set; }
        public int HHConsultorC { get; set; }
        public decimal Costosocios { get; set; }
        public decimal Costostaff { get; set; }
        public decimal CostoconsultorA { get; set; }
        public decimal CostoconsultorB { get; set; }
        public decimal CostoconsultorC { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal TotalGastos { get; set; }
    }
}
