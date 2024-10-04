using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Segmento
    {
        public Segmento()
        {
            Facturas = new HashSet<Factura>();
            ProyectoGastos = new HashSet<ProyectoGasto>();
            ProyectoServicios = new HashSet<ProyectoServicio>();
            SegmentoCcostos = new HashSet<SegmentoCcosto>();
            UsuarioProyectos = new HashSet<UsuarioProyecto>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string TipoSegmento { get; set; } = null!;
        public int IdCuenta { get; set; }

        public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
        public virtual ICollection<Factura> Facturas { get; set; }
        public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; }
        public virtual ICollection<ProyectoServicio> ProyectoServicios { get; set; }
        public virtual ICollection<SegmentoCcosto> SegmentoCcostos { get; set; }
        public virtual ICollection<UsuarioProyecto> UsuarioProyectos { get; set; }
    }
}
