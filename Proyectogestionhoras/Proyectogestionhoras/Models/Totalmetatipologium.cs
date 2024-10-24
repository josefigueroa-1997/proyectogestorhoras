using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Totalmetatipologium
    {
        public int Id { get; set; }
        public int? Anio { get; set; }
        public int? Totalproyecto { get; set; }
        public decimal? Totalporproyecto { get; set; }
        public int? Duracionmedia { get; set; }
        public decimal? Totalmensuales { get; set; }
    }
}
