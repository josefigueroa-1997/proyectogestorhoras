using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class UsuarioProyecto
    {
        public int Id { get; set; }
        public int IdProyecto { get; set; }
        public int IdUsuario { get; set; }
        public decimal? HhStaff { get; set; }
        public decimal? HhConsultora { get; set; }
        public decimal? HhConsultorb { get; set; }
        public decimal? HhConsultorc { get; set; }
        public decimal? HhSocios { get; set; }
        public decimal? CostoUnitarioAsignado { get; set; }
        public int? Idsegmento { get; set; }
        public decimal? Costoconsultora { get; set; }
        public decimal? Costoconsultorb { get; set; }
        public decimal? Costoconsultorc { get; set; }

        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual Segmento? IdsegmentoNavigation { get; set; }
    }
}
