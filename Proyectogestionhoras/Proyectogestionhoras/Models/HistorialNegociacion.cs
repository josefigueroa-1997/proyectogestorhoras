using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class HistorialNegociacion
    {
        public int Id { get; set; }
        public int? IdProyecto { get; set; }
        public string? Afectaiva { get; set; }
        public decimal? Monto { get; set; }
        public int? Plazo { get; set; }
        public DateTime? Fechainicio { get; set; }
        public DateTime? Fechatermino { get; set; }
        public DateTime? Fechanegociacion { get; set; }
        public string? Probabilidad { get; set; }
        public decimal? Nivelprobabilidad { get; set; }
        public int? Hhsocios { get; set; }
        public int? Hhstaff { get; set; }
        public int? Hhconsultora { get; set; }
        public int? Hhconsultorb { get; set; }
        public int? Hhconsultorc { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Costosocio { get; set; }
        public decimal? Costostaff { get; set; }
        public decimal? Costoconsultora { get; set; }
        public decimal? Costoconsultorb { get; set; }
        public decimal? Costoconsultorc { get; set; }

        public virtual Proyecto? IdProyectoNavigation { get; set; }
    }
}
