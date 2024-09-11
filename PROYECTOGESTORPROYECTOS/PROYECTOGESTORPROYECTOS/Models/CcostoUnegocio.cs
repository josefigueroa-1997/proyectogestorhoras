using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class CcostoUnegocio
{
    public int Id { get; set; }

    public int? IdCcosto { get; set; }

    public int? IdUnegocio { get; set; }

    public string? Codigo { get; set; }

    public virtual Ccosto? IdCcostoNavigation { get; set; }

    public virtual Unegocio? IdUnegocioNavigation { get; set; }

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
