using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Ccosto
{
    public int Id { get; set; }

    public string TipoCcosto { get; set; } = null!;

    public virtual ICollection<CcostoUnegocio> CcostoUnegocios { get; set; } = new List<CcostoUnegocio>();
}
