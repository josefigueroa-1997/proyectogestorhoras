using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Proveedore
    {
        public int Id { get; set; }
        public string? Rut { get; set; }
        public string? Nombre { get; set; }
        public string? Funcion { get; set; }
        public string? Tipo { get; set; }
    }
}
