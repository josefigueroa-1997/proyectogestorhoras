using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Historialpresupuesto
    {
        public int Id { get; set; }
        public int? Idproyecto { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Costosocio { get; set; }
        public decimal? Costostaff { get; set; }
        public decimal? Costoconsultora { get; set; }
        public decimal? Costoconsultorb { get; set; }
        public decimal? Costoconsultorc { get; set; }
        public string? Afectaiva { get; set; }

        public virtual Proyecto? IdproyectoNavigation { get; set; }
    }
}
