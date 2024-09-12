using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class SegmentoCosto
    {
        public SegmentoCosto()
        {
            Recursos = new HashSet<Recurso>();
        }

        public int Id { get; set; }
        public string Cuenta { get; set; } = null!;
        public int IdCuenta { get; set; }
        public int? IdHonorariosExternos { get; set; }

        public virtual ICollection<Recurso> Recursos { get; set; }
    }
}
