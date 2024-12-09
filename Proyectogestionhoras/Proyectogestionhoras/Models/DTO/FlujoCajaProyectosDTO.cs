namespace Proyectogestionhoras.Models.DTO
{
    public class FlujoCajaProyectosDTO
    {
        public string? Tipo { get; set; }
        public string? NumProyecto { get; set; }
        public string? NombreProyecto { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int IdCuenta { get; set; }
        public string? Cuenta { get; set; }
        public decimal Monto { get; set; }
        public string? Estado { get; set; }
        public int Orden { get; set; }
    }
}
