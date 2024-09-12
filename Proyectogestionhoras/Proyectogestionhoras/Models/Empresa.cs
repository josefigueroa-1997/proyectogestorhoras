using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Empresa
    {
        public Empresa()
        {
            Proyectos = new HashSet<Proyecto>();
        }

        public int Id { get; set; }
        public string TipoEmpresa { get; set; } = null!;

        public virtual ICollection<Proyecto> Proyectos { get; set; }
    }
}
