namespace Proyectogestionhoras.Models.DTO
{
    public class FlujoCajaDTO
    {
        public string? NumProyecto {  get; set; }
        public string? Nombre {  get; set; }
        public int Mes {  get; set; }
        public int Anio {  get; set; }
        public decimal? MontoProyectado {  get; set; }
        public decimal? MontoReal {  get; set; }
        public int Idcuenta { get; set; }
        public string? Cuenta { get; set; }
        public decimal? MontoPorCobrar { get; set; }


    }
}
