using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Unegocio
    {
        public Unegocio()
        {
            CcostoUnegocios = new HashSet<CcostoUnegocio>();
        }

        public int Id { get; set; }
        public string TipoUnegocio { get; set; } = null!;

        public virtual ICollection<CcostoUnegocio> CcostoUnegocios { get; set; }
    }
}
