﻿namespace Proyectogestionhoras.Models.DTO
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

        public string? Estado { get; set; }
        public decimal? MontoProyectado { get; set; }

    }
}
