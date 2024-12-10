using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Actividade
    {
        public Actividade()
        {
            Subactividads = new HashSet<Subactividad>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? TipoAcatividad { get; set; }
        public string? Controlhh { get; set; }

        public virtual ICollection<Subactividad> Subactividads { get; set; }
    }
}
