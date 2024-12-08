namespace Proyectogestionhoras.Models.ViewModel
{
    public class GastoViewModel
    {
        public int IdGastoProyecto { get; set; }
        public int Idgastos { get; set; }
        public int IdSegmento { get; set; }
        public decimal MontoGasto { get; set; }
        public DateTime Fecha { get; set; }
        public bool EsEliminado { get; set; }
        public string? espresupuesto { get; set; }
    }
}
