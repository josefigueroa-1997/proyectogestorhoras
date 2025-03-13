using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class ProyectoGasto
    {
        public int Id { get; set; }
        public int IdProyecto { get; set; }
        public int IdGastos { get; set; }
        public decimal? Monto { get; set; }
        public int? Idsegmento { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Espresupuesto { get; set; }

        public virtual Gasto IdGastosNavigation { get; set; } = null!;
        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Segmento? IdsegmentoNavigation { get; set; }
    }
}
