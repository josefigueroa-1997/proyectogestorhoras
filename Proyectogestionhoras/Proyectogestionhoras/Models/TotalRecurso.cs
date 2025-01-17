﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class TotalRecurso
    {
        public int Id { get; set; }
        public string TipoRecurso { get; set; } = null!;
        public decimal TotalHhAnuales { get; set; }
        public int? Anio { get; set; }
        public decimal? Totalinmodificable { get; set; }
        public int? Cantidad { get; set; }
    }
}
