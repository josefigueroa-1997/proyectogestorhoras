using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Tipologium
    {
        public Tipologium()
        {
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public string TipoTipologia { get; set; } = null!;
        public int? Hhsocios { get; set; }
        public int? Hhstaff { get; set; }
        public int? Consultora { get; set; }
        public int? Consutlrob { get; set; }
        public int? Consultorc { get; set; }
        public int? Desde { get; set; }
        public int? Hasta { get; set; }
        public decimal Monto { get; set; }

        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
