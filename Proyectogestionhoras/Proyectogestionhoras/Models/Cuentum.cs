using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Cuentum
    {
        public Cuentum()
        {
            Segmentos = new HashSet<Segmento>();
            Subactividads = new HashSet<Subactividad>();
        }

        public int Id { get; set; }
        public int Idcuenta { get; set; }
        public string Cuenta { get; set; } = null!;
        public int? Idtipocuenta { get; set; }

        public virtual Tipocuentum? IdtipocuentaNavigation { get; set; }
        public virtual ICollection<Segmento> Segmentos { get; set; }
        public virtual ICollection<Subactividad> Subactividads { get; set; }
    }
}
