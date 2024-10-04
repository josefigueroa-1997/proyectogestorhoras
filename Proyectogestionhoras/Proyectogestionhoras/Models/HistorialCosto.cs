using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class HistorialCosto
    {
        public int Id { get; set; }
        public int? IdUsuario { get; set; }
        public decimal? CostoUnitario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}
