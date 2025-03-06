using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Recurso
    {
        public Recurso()
        {
            Bonosocios = new HashSet<Bonosocio>();
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string NombreRecurso { get; set; } = null!;
        public decimal? CostoUnitario { get; set; }
        public decimal? ProcentajeProyecto { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? NumeroHoras { get; set; }
        public decimal? HhMensuales { get; set; }
        public decimal? HhAnuales { get; set; }
        public string? TipoConsultor { get; set; }
        public decimal? TotalSocios { get; set; }
        public decimal? TotalStaff { get; set; }
        public string? Estado { get; set; }

        public virtual ICollection<Bonosocio> Bonosocios { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
