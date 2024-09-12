using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Ccosto
    {
        public Ccosto()
        {
            CcostoUnegocios = new HashSet<CcostoUnegocio>();
        }

        public int Id { get; set; }
        public string TipoCcosto { get; set; } = null!;

        public virtual ICollection<CcostoUnegocio> CcostoUnegocios { get; set; }
    }
}
