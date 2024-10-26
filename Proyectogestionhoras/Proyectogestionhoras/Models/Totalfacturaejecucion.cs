using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Totalfacturaejecucion
    {
        public int Id { get; set; }
        public int? Anio { get; set; }
        public int? Trimestre { get; set; }
        public decimal? Total { get; set; }
    }
}
