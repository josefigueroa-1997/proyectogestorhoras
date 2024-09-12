using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class CcostoUnegocio
    {
        public CcostoUnegocio()
        {
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public int? IdCcosto { get; set; }
        public int? IdUnegocio { get; set; }
        public string? Codigo { get; set; }

        public virtual Ccosto? IdCcostoNavigation { get; set; }
        public virtual Unegocio? IdUnegocioNavigation { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
