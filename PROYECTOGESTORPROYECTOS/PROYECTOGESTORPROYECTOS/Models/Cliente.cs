using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string Pais { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? PagWeb { get; set; }

    public string? Linkedin { get; set; }

    public string? Instagram { get; set; }

    public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
