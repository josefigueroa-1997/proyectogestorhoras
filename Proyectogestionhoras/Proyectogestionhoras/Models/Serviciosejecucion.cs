using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Serviciosejecucion
    {
        public int Id { get; set; }
        public int? Idservicio { get; set; }
        public int? Idproveedor { get; set; }
        public int? Idproyecto { get; set; }
        public decimal? Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Observacion { get; set; }

        public virtual Proveedore? IdproveedorNavigation { get; set; }
        public virtual Proyecto? IdproyectoNavigation { get; set; }
    }
}
