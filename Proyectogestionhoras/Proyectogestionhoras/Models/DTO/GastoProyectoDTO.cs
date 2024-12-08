namespace Proyectogestionhoras.Models.DTO
{
    public class GastoProyectoDTO
    {
        public int Id { get; set; }
        public int IDGASTOS { get; set; }
        public string? NOMBREGASTO { get; set; }
        public string? NOMBRSEGMENTO { get; set; }
        public string? CUENTA { get; set; }
        public int IDCUENTA { get; set; }
        public decimal? MONTO { get; set; }
        public int IDSEGMENTO { get; set; }
        public DateTime? FECHA { get; set; }

        public string? Espresupuesto { get; set; }
    }
}
