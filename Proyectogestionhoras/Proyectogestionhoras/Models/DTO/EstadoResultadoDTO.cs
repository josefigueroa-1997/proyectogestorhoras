namespace Proyectogestionhoras.Models.DTO
{
    public class EstadoResultadoDTO
    {
        public string? tipo { get; set; }
       
        public int mes { get; set; }
        public int anio { get; set; }
        public int idCuenta { get; set; }
        public string? cuenta { get; set; }
        public decimal monto { get; set; }
      
    }
}
