namespace Proyectogestionhoras.Models.DTO
{
    public class TotalesServiciosDTO
    {

        public int IDCUENTA { get; set; }
        public int IDSERVICIO { get; set; }
        public string CUENTA { get; set; }
        public string NOMBRESERVICIO { get; set; }
        public decimal? MontoProyectado { get; set; }  // Presupuesto
        public decimal TotalReal { get; set; }        // Suma de montos con estado "Pagado"
        public decimal TotalForecast { get; set; }    // Suma de montos con estado "Forecast"
    }
}
