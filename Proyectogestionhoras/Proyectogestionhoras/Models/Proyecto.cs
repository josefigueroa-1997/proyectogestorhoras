using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Proyecto
    {
        public Proyecto()
        {
            Facturas = new HashSet<Factura>();
            HhUsuarioHistorials = new HashSet<HhUsuarioHistorial>();
            HistorialNegociacions = new HashSet<HistorialNegociacion>();
            ProyectoGastos = new HashSet<ProyectoGasto>();
            ProyectoServicios = new HashSet<ProyectoServicio>();
            UsuarioProyectos = new HashSet<UsuarioProyecto>();
        }

        public int Id { get; set; }
        public int IdPresupuesto { get; set; }
        public int IdTipologia { get; set; }
        public string Nombre { get; set; } = null!;
        public string NumProyecto { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public int Plazo { get; set; }
        public int TipoEmpresa { get; set; }
        public int? IdCcostoUnegocio { get; set; }
        public int IdClienteSucursal { get; set; }
        public int StatusProyecto { get; set; }
        public string? Probabilidad { get; set; }
        public decimal? PorcentajeProbabilidad { get; set; }
        public DateTime? FechaPlazoNeg { get; set; }

        public virtual CcostoUnegocio? IdCcostoUnegocioNavigation { get; set; }
        public virtual SucursalCliente IdClienteSucursalNavigation { get; set; } = null!;
        public virtual Presupuesto IdPresupuestoNavigation { get; set; } = null!;
        public virtual Tipologium IdTipologiaNavigation { get; set; } = null!;
        public virtual StatusProyecto StatusProyectoNavigation { get; set; } = null!;
        public virtual Empresa TipoEmpresaNavigation { get; set; } = null!;
        public virtual ICollection<Factura> Facturas { get; set; }
        public virtual ICollection<HhUsuarioHistorial> HhUsuarioHistorials { get; set; }
        public virtual ICollection<HistorialNegociacion> HistorialNegociacions { get; set; }
        public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; }
        public virtual ICollection<ProyectoServicio> ProyectoServicios { get; set; }
        public virtual ICollection<UsuarioProyecto> UsuarioProyectos { get; set; }
    }
}
