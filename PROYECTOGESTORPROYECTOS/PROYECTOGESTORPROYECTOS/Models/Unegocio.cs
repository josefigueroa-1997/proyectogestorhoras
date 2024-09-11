using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Unegocio
{
    public int Id { get; set; }

    public string TipoUnegocio { get; set; } = null!;

    public virtual ICollection<CcostoUnegocio> CcostoUnegocios { get; set; } = new List<CcostoUnegocio>();
}
