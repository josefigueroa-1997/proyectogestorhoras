using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Contacto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public DateOnly Desde { get; set; }

    public DateOnly Hasta { get; set; }

    public string Email { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public int IdCliente { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
