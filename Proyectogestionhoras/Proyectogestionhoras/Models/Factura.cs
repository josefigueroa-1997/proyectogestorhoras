using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Factura
    {
        public Factura()
        {
            FacturaProyectos = new HashSet<FacturaProyecto>();
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public virtual ICollection<FacturaProyecto> FacturaProyectos { get; set; }
    }
}
