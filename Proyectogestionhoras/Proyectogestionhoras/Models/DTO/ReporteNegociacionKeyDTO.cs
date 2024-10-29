namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteNegociacionKeyDTO
    {

        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; }
        public int HHSocios { get; set; }
        public decimal Monto { get; set; }
        public string? Probabilidad { get; set; }
        public DateTime FechaPlazo { get; set; }


    }
}
