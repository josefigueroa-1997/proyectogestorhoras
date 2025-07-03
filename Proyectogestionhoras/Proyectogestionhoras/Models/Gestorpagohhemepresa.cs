using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Gestorpagohhemepresa
    {
        public int Id { get; set; }
        public int? Idplanillaempresa { get; set; }
        public DateTime? Fechapago { get; set; }
        public int? Estado { get; set; }

        public virtual PlanillaRegistroEmpresa? IdplanillaempresaNavigation { get; set; }
    }
}
