using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class PlanillaRegistroEmpresa
    {
        public int Id { get; set; }
        public int IdPlanilla { get; set; }
        public decimal? Hhregistradas { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? Fecharegistro { get; set; }
        public int Idsubactividad { get; set; }
        public decimal? CostoMonetario { get; set; }

        public virtual Planilla IdPlanillaNavigation { get; set; } = null!;
        public virtual Subactividad IdsubactividadNavigation { get; set; } = null!;
    }
}
