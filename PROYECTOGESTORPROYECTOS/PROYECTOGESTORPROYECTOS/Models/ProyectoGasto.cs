using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class ProyectoGasto
{
    public int Id { get; set; }

    public int IdProyecto { get; set; }

    public int IdGastos { get; set; }

    public virtual Gasto IdGastosNavigation { get; set; } = null!;

    public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
}
