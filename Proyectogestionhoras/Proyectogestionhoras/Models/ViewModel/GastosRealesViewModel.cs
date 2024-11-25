namespace Proyectogestionhoras.Models.ViewModel
{
    public class GastosRealesViewModel
    {
        public int IdGastoReal { get; set; }
        public int? Idgasto { get; set; }
        public int? Idproveedor { get; set; }
       
        public int? Segmento { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Observacion { get; set; }
    }
}
