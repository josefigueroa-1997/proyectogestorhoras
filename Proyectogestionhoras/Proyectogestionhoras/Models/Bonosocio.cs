using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Bonosocio
    {
        public int Id { get; set; }
        public int Idbono { get; set; }
        public int Idrecurso { get; set; }
        public int? Anio { get; set; }
        public int? Trimeste { get; set; }

        public virtual Bono IdbonoNavigation { get; set; } = null!;
        public virtual Recurso IdrecursoNavigation { get; set; } = null!;
    }
}
