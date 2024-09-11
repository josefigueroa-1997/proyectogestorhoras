using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Factura
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Monto { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<FacturaProyecto> FacturaProyectos { get; set; } = new List<FacturaProyecto>();
}
