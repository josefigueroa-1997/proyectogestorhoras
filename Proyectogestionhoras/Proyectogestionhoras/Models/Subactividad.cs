using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Subactividad
    {
        public Subactividad()
        {
            PlanillaRegistroEmpresas = new HashSet<PlanillaRegistroEmpresa>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int? Idactividad { get; set; }

        public virtual Actividade? IdactividadNavigation { get; set; }
        public virtual ICollection<PlanillaRegistroEmpresa> PlanillaRegistroEmpresas { get; set; }
    }
}
