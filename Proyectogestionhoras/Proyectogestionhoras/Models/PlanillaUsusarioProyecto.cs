using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class PlanillaUsusarioProyecto
    {
        public int Id { get; set; }
        public int IdPlanilla { get; set; }
        public int IdUsuProy { get; set; }
        public int RegistroHhProyecto { get; set; }
        public int? IdActividad { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual Actividade? IdActividadNavigation { get; set; }
        public virtual Planilla IdPlanillaNavigation { get; set; } = null!;
    }
}
