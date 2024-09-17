using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Sucursal
    {
        public Sucursal()
        {
            SucursalClientes = new HashSet<SucursalCliente>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<SucursalCliente> SucursalClientes { get; set; }
    }
}
