using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Proveedore
    {
        public Proveedore()
        {
            Gastosejecucions = new HashSet<Gastosejecucion>();
            Serviciosejecucions = new HashSet<Serviciosejecucion>();
        }

        public int Id { get; set; }
        public string? Rut { get; set; }
        public string? Nombre { get; set; }
        public string? Funcion { get; set; }
        public string? Tipo { get; set; }

        public virtual ICollection<Gastosejecucion> Gastosejecucions { get; set; }
        public virtual ICollection<Serviciosejecucion> Serviciosejecucions { get; set; }
    }
}
