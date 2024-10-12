using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Actividade
    {
        public Actividade()
        {
            PlanillaUsusarioProyectos = new HashSet<PlanillaUsusarioProyecto>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? TipoAcatividad { get; set; }

        public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; }
    }
}
