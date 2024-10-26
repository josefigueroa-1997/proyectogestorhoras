using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Bono
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public int Anio { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? Valorreal { get; set; }
        public decimal? Valorfinal { get; set; }
    }
}
