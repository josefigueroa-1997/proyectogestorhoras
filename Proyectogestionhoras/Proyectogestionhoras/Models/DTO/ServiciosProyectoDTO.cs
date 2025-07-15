namespace Proyectogestionhoras.Models.DTO
{
    public class ServiciosProyectoDTO
    {
        public int Id { get; set; }
        public int IDSERVICIO { get; set; }
        public string? NOMBRESERVICIO { get; set; }
        public string? NOMBRSEGMENTO { get; set; }
        public string? CUENTA { get; set; }
        public int IDCUENTA { get; set; }
        public decimal? MONTO { get; set; }
        public DateTime? FECHA { get; set; }
        public int idproveedor { get; set; }
        public int idservicioreal { get; set; }
        public int idsegmento { get; set; }
        public string? Estado { get; set; }
        public string? observacion { get; set; }
        public string? tipo { get; set; }
        public decimal? MontoProyectado { get; set; }

    }
}
