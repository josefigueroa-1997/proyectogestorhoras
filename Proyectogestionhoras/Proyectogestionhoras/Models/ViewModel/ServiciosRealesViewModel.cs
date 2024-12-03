namespace Proyectogestionhoras.Models.ViewModel
{
    public class ServiciosRealesViewModel
    {
        public int IdServicioReal { get; set; }
        public int? Idservicio { get; set; }
        public int? Idproveedor { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Observacion { get; set; }
        public string? Estado { get; set; }
    }
}
