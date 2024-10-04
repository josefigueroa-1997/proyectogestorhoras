using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class UsuarioProyecto
    {
        public int Id { get; set; }
        public int IdProyecto { get; set; }
        public int IdUsuario { get; set; }
        public int? HhStaff { get; set; }
        public int? HhConsultora { get; set; }
        public int? HhConsultorb { get; set; }
        public int? HhConsultorc { get; set; }
        public int? HhSocios { get; set; }
        public decimal? CostoUnitarioAsignado { get; set; }
        public int? Idsegmento { get; set; }

        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual Segmento? IdsegmentoNavigation { get; set; }
    }
}
