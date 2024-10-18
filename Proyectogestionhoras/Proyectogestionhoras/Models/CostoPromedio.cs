using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class CostoPromedio
    {
        public int Id { get; set; }
        public string? TipoRecurso { get; set; }
        public decimal? Valor { get; set; }
    }
}
