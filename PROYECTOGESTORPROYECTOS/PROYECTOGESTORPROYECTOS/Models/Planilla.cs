using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Planilla
{
    public int Id { get; set; }

    public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; } = new List<PlanillaUsusarioProyecto>();
}
