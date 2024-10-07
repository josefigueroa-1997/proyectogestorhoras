namespace Proyectogestionhoras.Models.DTO
{
    public class GastoProyectoDTO
    {
        public int IDGASTOS { get; set; }
        public string? NOMBREGASTO { get; set; }
        public string? NOMBRSEGMENTO { get; set; }
        public string? CUENTA { get; set; }
        public int IDCUENTA { get; set; }
        public decimal? MONTO { get; set; }
    }
}
