using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Recurso
{
    public int Id { get; set; }

    public string NombreRecurso { get; set; } = null!;

    public int NumeroHoras { get; set; }

    public decimal? CostoUnitario { get; set; }

    public int? IdSegmentocostos { get; set; }

    public virtual SegmentoCosto? IdSegmentocostosNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
