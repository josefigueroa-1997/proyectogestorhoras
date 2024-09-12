using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Recurso
    {
        public Recurso()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string NombreRecurso { get; set; } = null!;
        public int NumeroHoras { get; set; }
        public decimal? CostoUnitario { get; set; }
        public int? IdSegmentocostos { get; set; }

        public virtual SegmentoCosto? IdSegmentocostosNavigation { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
