using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class SegmentoCcosto
    {
        public int Id { get; set; }
        public int IdSegmento { get; set; }
        public int IdCcosto { get; set; }

        public virtual CcostoUnegocio IdCcostoNavigation { get; set; } = null!;
        public virtual Segmento IdSegmentoNavigation { get; set; } = null!;
    }
}
