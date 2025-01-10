using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Cuota
    {
        public int Id { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal? Montocuota { get; set; }
        public string? Observacion { get; set; }
        public int? Idpresupuesto { get; set; }

        public virtual Presupuesto? IdpresupuestoNavigation { get; set; }
    }
}
