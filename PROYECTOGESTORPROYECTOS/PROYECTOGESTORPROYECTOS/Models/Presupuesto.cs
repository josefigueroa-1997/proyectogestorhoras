using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Presupuesto
{
    public int Id { get; set; }

    public decimal Monto { get; set; }

    public string Moneda { get; set; } = null!;

    public string Afectaiva { get; set; } = null!;

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
