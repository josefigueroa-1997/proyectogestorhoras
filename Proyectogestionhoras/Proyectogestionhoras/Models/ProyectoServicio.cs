﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class ProyectoServicio
    {
        public int Id { get; set; }
        public int IdProyecto { get; set; }
        public int IdServicio { get; set; }
        public int IdCuenta { get; set; }
        public decimal Monto { get; set; }

        public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
        public virtual Servicio IdServicioNavigation { get; set; } = null!;
    }
}