using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Segmento
    {
        public Segmento()
        {
            SegmentoCcostos = new HashSet<SegmentoCcosto>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string TipoSegmento { get; set; } = null!;
        public int IdCuenta { get; set; }

        public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
        public virtual ICollection<SegmentoCcosto> SegmentoCcostos { get; set; }
    }
}
