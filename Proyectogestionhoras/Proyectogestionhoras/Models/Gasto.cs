using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Gasto
    {
        public Gasto()
        {
            ProyectoGastos = new HashSet<ProyectoGasto>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int? Idcuenta { get; set; }

        public virtual ICollection<ProyectoGasto> ProyectoGastos { get; set; }
    }
}
