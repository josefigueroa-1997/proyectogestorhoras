namespace Proyectogestionhoras.Models.DTO
{
    public class GastosHHRecursosDTO
    {
        public int idcuentasocio { get; set; }
        public int idcuentastaff { get; set; }
        public int idgastohh { get; set; }
        public string? cuentasocio { get; set; }
        public string? cuentastaff { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public string? tiporecurso { get; set; }
        public decimal totalhh { get; set; }
        public decimal? costorecursosocio { get; set; } 
        public decimal? costorecurstaff { get; set; } 
        public decimal? costorecurso { get; set; } 
        public decimal costoconsultoresexternos { get; set; }
        public string? nombreproyecto { get; set; }
    }
}
