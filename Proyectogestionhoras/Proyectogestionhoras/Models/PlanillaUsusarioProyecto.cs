using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class PlanillaUsusarioProyecto
    {
        public int Id { get; set; }
        public int IdPlanilla { get; set; }
        public int IdUsuProy { get; set; }

        public virtual Planilla IdPlanillaNavigation { get; set; } = null!;
        public virtual UsuarioProyecto IdUsuProyNavigation { get; set; } = null!;
    }
}
