using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Planilla
    {
        public Planilla()
        {
            PlanillaUsusarioProyectos = new HashSet<PlanillaUsusarioProyecto>();
        }

        public int Id { get; set; }

        public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; }
    }
}
