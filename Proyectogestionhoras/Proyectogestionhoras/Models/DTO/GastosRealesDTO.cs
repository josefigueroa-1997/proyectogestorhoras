namespace Proyectogestionhoras.Models.DTO
{
    public class GastosRealesDTO
    {
        public int IdGastosReal { get; set; }
        public int? IdGasto { get; set; }
        public int? IdProveedor { get; set; }
        public int IdSegmento { get; set; }
        public string? NombreSegmento { get; set; }
        public string? Cuenta { get; set; }
        public int IdCuenta { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
