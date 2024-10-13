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
        public int IdUsuario { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; }
    }
}
