using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Proyecto
    {
        public Proyecto()
        {
            Facturas = new HashSet<Factura>();
            Gastosejecucions = new HashSet<Gastosejecucion>();
            HhUsuarioHistorials = new HashSet<HhUsuarioHistorial>();
            HistorialCostosProyectos = new HashSet<HistorialCostosProyecto>();
            HistorialNegociacions = new HashSet<HistorialNegociacion>();
            Historialpresupuestos = new HashSet<Historialpresupuesto>();
            Ingresosreales = new HashSet<Ingresosreale>();
            ProyectoGastos = new HashSet<ProyectoGasto>();
            ProyectoServicios = new HashSet<ProyectoServicio>();
            Serviciosejecucions = new HashSet<Serviciosejecucion>();
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
        public virtual ICollection<Gastosejecucion> Gastosejecucions { get; set; }
        public virtual ICollection<HhUsuarioHistorial> HhUsuarioHistorials { get; set; }
        public virtual ICollection<HistorialCostosProyecto> HistorialCostosProyectos { get; set; }
        public virtual ICollection<HistorialNegociacion> HistorialNegociacions { get; set; }
        public virtual ICollection<Historialpresupuesto> Historialpresupuestos { get; set; }
        public virtual ICollection<Ingresosreale> Ingresosreales { get; set; }
        public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; }
        public virtual ICollection<ProyectoServicio> ProyectoServicios { get; set; }
        public virtual ICollection<Serviciosejecucion> Serviciosejecucions { get; set; }
        public virtual ICollection<UsuarioProyecto> UsuarioProyectos { get; set; }
    }
}
