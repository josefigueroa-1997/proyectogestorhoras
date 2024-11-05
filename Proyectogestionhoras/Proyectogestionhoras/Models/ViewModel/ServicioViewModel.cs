namespace Proyectogestionhoras.Models.ViewModel
{
    public class ServicioViewModel
    {
        public int Idservicios { get; set; }     
        public int IdSegmento { get; set; }        
        public decimal MontoServicio { get; set; }
        public DateTime Fecha { get; set; }
        public bool EsEliminado { get; set; }

    }
}
