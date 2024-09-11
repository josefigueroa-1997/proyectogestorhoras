using System;
using System.Collections.Generic;

namespace PROYECTOGESTORPROYECTOS.Models;

public partial class Proyecto
{
    public int Id { get; set; }

    public int IdPresupuesto { get; set; }

    public int IdTipologia { get; set; }

    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string NumProyecto { get; set; } = null!;

    public string TipoStatus { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaTermino { get; set; }

    public int Plazo { get; set; }

    public int TipoEmpresa { get; set; }

    public int? IdCcostoUnegocio { get; set; }

    public virtual ICollection<FacturaProyecto> FacturaProyectos { get; set; } = new List<FacturaProyecto>();

    public virtual CcostoUnegocio? IdCcostoUnegocioNavigation { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Presupuesto IdPresupuestoNavigation { get; set; } = null!;

    public virtual Tipologium IdTipologiaNavigation { get; set; } = null!;

    public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; } = new List<ProyectoGasto>();

    public virtual Empresa TipoEmpresaNavigation { get; set; } = null!;

    public virtual ICollection<UsuarioProyecto> UsuarioProyectos { get; set; } = new List<UsuarioProyecto>();
}
