using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class StatusProyecto
    {
        public StatusProyecto()
        {
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public string? TipoStatus { get; set; }

        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
