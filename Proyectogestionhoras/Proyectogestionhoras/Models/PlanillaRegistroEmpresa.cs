﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class PlanillaRegistroEmpresa
    {
        public PlanillaRegistroEmpresa()
        {
            Gestorpagohhemepresas = new HashSet<Gestorpagohhemepresa>();
        }

        public int Id { get; set; }
        public int IdPlanilla { get; set; }
        public decimal? Hhregistradas { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? Fecharegistro { get; set; }
        public int Idsubactividad { get; set; }
        public decimal? CostoMonetario { get; set; }
        public string? Correlativo { get; set; }

        public virtual Planilla IdPlanillaNavigation { get; set; } = null!;
        public virtual Subactividad IdsubactividadNavigation { get; set; } = null!;
        public virtual ICollection<Gestorpagohhemepresa> Gestorpagohhemepresas { get; set; }
    }
}
