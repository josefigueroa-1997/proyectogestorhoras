using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Presupuesto
    {
        public Presupuesto()
        {
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = null!;
        public string Afectaiva { get; set; } = null!;
        public decimal? Montoiva { get; set; }

        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
