namespace Proyectogestionhoras.Models.DTO
{
    public class ProyectoDTO
    {
        public int Id { get; set; }
        public string? numproyecto { get; set; }
        public string? Tipo_Unegocio { get; set; }
        public string? Tipo_CCosto { get; set; }
        public string? Codigo { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; }
        public string? Tipo_Tipologia { get; set; }
        public string? Tipo_Empresa { get; set; }
        public string? AfectaIva { get; set; }
        public string? Tipo_Status { get; set; }
        public string? Probabilidad { get; set; }
        public decimal? Porcentaje_Probabilidad { get; set; }
        public int Plazo { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Termino { get; set;}
        public DateTime? Fecha_Plazo_Neg { get; set;}


    }
}
