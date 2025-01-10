using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Presupuesto
    {
        public Presupuesto()
        {
            Cuota = new HashSet<Cuota>();
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public decimal? Monto { get; set; }
        public string Moneda { get; set; } = null!;
        public string Afectaiva { get; set; } = null!;
        public decimal? Montoiva { get; set; }
        public decimal? Montomonedaorigen { get; set; }
        public decimal? Tasacambio { get; set; }
        public int? Cantidadcuotas { get; set; }

        public virtual ICollection<Cuota> Cuota { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
