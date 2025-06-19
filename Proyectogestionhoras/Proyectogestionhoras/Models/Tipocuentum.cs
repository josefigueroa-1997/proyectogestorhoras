using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Tipocuentum
    {
        public Tipocuentum()
        {
            Cuenta = new HashSet<Cuentum>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<Cuentum> Cuenta { get; set; }
    }
}
