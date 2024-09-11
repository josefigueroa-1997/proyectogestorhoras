using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class SegmentoCosto
{
    public int Id { get; set; }

    public string Cuenta { get; set; } = null!;

    public int IdCuenta { get; set; }

    public int? IdHonorariosExternos { get; set; }

    public virtual ICollection<Recurso> Recursos { get; set; } = new List<Recurso>();
}
