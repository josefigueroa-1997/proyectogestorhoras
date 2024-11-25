using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Servicio
    {
        public Servicio()
        {
            ProyectoServicios = new HashSet<ProyectoServicio>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int? Idcuenta { get; set; }

        public virtual ICollection<ProyectoServicio> ProyectoServicios { get; set; }
    }
}
