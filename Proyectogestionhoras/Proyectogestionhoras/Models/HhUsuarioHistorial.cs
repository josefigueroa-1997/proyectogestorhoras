﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class HhUsuarioHistorial
    {
        public int Id { get; set; }
        public int? IdProyecto { get; set; }
        public int? HhSocios { get; set; }
        public int? HhStaff { get; set; }
        public int? HhConsultora { get; set; }
        public int? HhConsultorb { get; set; }
        public int? HhConsultorc { get; set; }

        public virtual Proyecto? IdProyectoNavigation { get; set; }
    }
}
