using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class UsuarioProyecto
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdProyecto { get; set; }

    public int HhSemanales { get; set; }

    public virtual Proyecto IdProyectoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; } = new List<PlanillaUsusarioProyecto>();
}
