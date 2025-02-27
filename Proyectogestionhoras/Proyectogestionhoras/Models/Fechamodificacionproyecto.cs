using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Fechamodificacionproyecto
    {
        public DateTime? Fechamodificacion { get; set; }
        public int Idproyecto { get; set; }
        public int Id { get; set; }

        public virtual Proyecto IdproyectoNavigation { get; set; } = null!;
    }
}
