using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class PlanillaUsusarioProyecto
    {
        public int Id { get; set; }
        public int IdPlanilla { get; set; }
        public int IdUsuProy { get; set; }
        public decimal? RegistroHhProyecto { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? Subactividad { get; set; }

        public virtual Planilla IdPlanillaNavigation { get; set; } = null!;
        public virtual Subactividad? SubactividadNavigation { get; set; }
    }
}
