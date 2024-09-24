using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Cuentum
    {
        public Cuentum()
        {
            Segmentos = new HashSet<Segmento>();
        }

        public int Id { get; set; }
        public int Idcuenta { get; set; }
        public string Cuenta { get; set; } = null!;

        public virtual ICollection<Segmento> Segmentos { get; set; }
    }
}
