﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Ingresosreale
    {
        public int Id { get; set; }
        public int? Cuota { get; set; }
        public int? Idproyecto { get; set; }
        public string? Numdocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal? Montous { get; set; }
        public decimal? Tc { get; set; }
        public decimal? Montoclp { get; set; }
        public decimal? Iva { get; set; }
        public string? Estado { get; set; }
        public int? Idcuenta { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? Observacion { get; set; }
        public string? Venta { get; set; }

        public virtual Proyecto? IdproyectoNavigation { get; set; }
    }
}
