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

        public virtual ICollection<ProyectoServicio> ProyectoServicios { get; set; }
    }
}
