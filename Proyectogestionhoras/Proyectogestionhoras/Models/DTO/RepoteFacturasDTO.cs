﻿namespace Proyectogestionhoras.Models.DTO
{
    public class RepoteFacturasDTO
    {
        public string? NumProyecto { get; set; }
        public string? Nombre { get; set; }
        public string? TipoStatus { get; set; }
        public decimal Neto { get; set; }
        public DateTime FechaFactura { get; set; }
        public string? TipoTipologia { get; set; }
        public string? NombreCliente { get; set; }
    }
}