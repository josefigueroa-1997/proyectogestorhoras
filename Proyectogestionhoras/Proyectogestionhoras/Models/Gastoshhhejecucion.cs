using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Gastoshhhejecucion
    {
        public int Id { get; set; }
        public int? Idproyecto { get; set; }
        public string? Tiporecurso { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public DateTime? Fechapago { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Hhtotales { get; set; }
        public string? Observacion { get; set; }

        public virtual Proyecto? IdproyectoNavigation { get; set; }
    }
}
