namespace Proyectogestionhoras.Models.DTO
{
    public class ReporteNegociacionDTO
    {
        public string? NumProyecto { get; set; }
        public string? Nombre { get; set; }
        public string? TipoTipologia { get; set; }
        public string? NombreCliente{ get; set; }
        public string? Probabilidad{ get; set; }
        public string? Unegocio{ get; set; }
        public string? CCosto{ get; set; }
        public DateTime FechaFactura{ get; set; }
        public decimal Neto { get; set; }   

    }
}
