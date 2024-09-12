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

        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
