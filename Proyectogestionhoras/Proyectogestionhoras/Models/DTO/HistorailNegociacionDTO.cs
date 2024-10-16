namespace Proyectogestionhoras.Models.DTO
{
    public class HistorailNegociacionDTO
    {
        public int IdNegociacion { get;set; }
        public int Id { get; set; }
        public string? NumProyecto { get;set; }
        public string? NombreProyecto { get; set; }
        public string? AfectaIva { get; set; }
        public decimal Monto { get; set; }
        public int Plazo { get; set; }
        public DateTime Fechainicio { get; set; } 
        public DateTime FechaTermino { get;set;}
        public DateTime FechaPlazoNeg { get; set; }
        public string? Probabilidad { get; set; }
        public decimal NivelProbabilidad { get; set; }
        public int Hhsocios { get; set; }
        public int Hhstaff { get; set; }
        public int Hhconsultora { get; set; }
        public int hhconsultorb { get; set; }
        public int hhconsultorc { get; set; }
        public DateTime Fecha { get; set; }
        public string? Tipologia { get; set; }
        public string? CentroCosto { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreDepartamento { get; set; }
        public decimal COSTOSOCIO { get; set; }
        public decimal CostoStaff { get; set; }
        public decimal CostoConsA { get; set; }
        public decimal CostoConsB { get; set; }
        public decimal CostoConsC { get; set; }

    }
}
