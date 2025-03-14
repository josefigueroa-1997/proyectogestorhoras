using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Historialcuentasproyecto
    {
        public int Id { get; set; }
        public int? Idcuentasocio { get; set; }
        public int? Idcuentastaff { get; set; }
        public string? Cuentasocio { get; set; }
        public string? Cuentastaff { get; set; }
        public int? Idproyecto { get; set; }

        public virtual Proyecto? IdproyectoNavigation { get; set; }
    }
}
