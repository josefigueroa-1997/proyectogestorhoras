using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Factura
    {
        public int Id { get; set; }
        public int IdProyecto { get; set; }
        public int Idsegmento { get; set; }

        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Segmento IdsegmentoNavigation { get; set; } = null!;
    }
}
