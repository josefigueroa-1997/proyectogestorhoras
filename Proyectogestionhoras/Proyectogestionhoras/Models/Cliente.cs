using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Contactos = new HashSet<Contacto>();
            Proyectos = new HashSet<Proyecto>();
            SucursalClientes = new HashSet<SucursalCliente>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string Pais { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? PagWeb { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string IdCliente { get; set; } = null!;

        public virtual ICollection<Contacto> Contactos { get; set; }
        public virtual ICollection<Proyecto> Proyectos { get; set; }
        public virtual ICollection<SucursalCliente> SucursalClientes { get; set; }
    }
}
