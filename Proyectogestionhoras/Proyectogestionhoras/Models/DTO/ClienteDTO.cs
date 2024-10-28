namespace Proyectogestionhoras.Models.DTO
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? PagWeb { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? Id_Cliente { get; set; }
        public string? Sucursales_Cliente { get; set; }
        public string? TipoCliente { get; set; }
        public List<int>? SucursalIds { get; set; }
    }
}
