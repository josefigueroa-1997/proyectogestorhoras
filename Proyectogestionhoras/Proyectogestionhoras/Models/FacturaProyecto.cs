using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class FacturaProyecto
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public int IdProyecto { get; set; }

        public virtual Factura IdFacturaNavigation { get; set; } = null!;
        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
    }
}
