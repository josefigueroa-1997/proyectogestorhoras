namespace Proyectogestionhoras.Models.DTO
{
    public class GastosHHRecursosDTO
    {
        public int idcuenta { get; set; }
        public string? cuenta { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public string? tiporecurso { get; set; }
        public decimal totalhh { get; set; }
        public decimal costorecurso { get; set; }
        public decimal costoconsultoresexternos { get; set; }
    }
}
