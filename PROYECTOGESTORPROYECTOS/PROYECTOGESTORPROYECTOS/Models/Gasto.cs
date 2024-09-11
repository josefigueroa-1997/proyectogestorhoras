using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Gasto
{
    public int Id { get; set; }

    public string TipoServicio { get; set; } = null!;

    public decimal Monto { get; set; }

    public string? Nombre { get; set; }

    public int? IdCuenta { get; set; }

    public string? Cuenta { get; set; }

    public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; } = new List<ProyectoGasto>();
}
