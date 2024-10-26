using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Bono
    {
        public Bono()
        {
            Bonosocios = new HashSet<Bonosocio>();
        }

        public int Id { get; set; }
        public decimal Monto { get; set; }
        public int Anio { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? Valorreal { get; set; }
        public decimal? Valorfinal { get; set; }

        public virtual ICollection<Bonosocio> Bonosocios { get; set; }
    }
}
