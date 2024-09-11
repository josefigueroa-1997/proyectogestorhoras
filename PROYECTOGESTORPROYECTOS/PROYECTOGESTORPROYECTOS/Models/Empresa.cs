using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Empresa
{
    public int Id { get; set; }

    public string TipoEmpresa { get; set; } = null!;

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
