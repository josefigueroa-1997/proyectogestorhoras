﻿using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class SucursalCliente
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; } = null!;
        public virtual Sucursal IdSucursalNavigation { get; set; } = null!;
    }
}