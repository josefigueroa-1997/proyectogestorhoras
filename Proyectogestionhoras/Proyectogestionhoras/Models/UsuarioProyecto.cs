using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class UsuarioProyecto
    {
        public UsuarioProyecto()
        {
            PlanillaUsusarioProyectos = new HashSet<PlanillaUsusarioProyecto>();
        }

        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdProyecto { get; set; }
        public int HhSemanales { get; set; }

        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<PlanillaUsusarioProyecto> PlanillaUsusarioProyectos { get; set; }
    }
}
