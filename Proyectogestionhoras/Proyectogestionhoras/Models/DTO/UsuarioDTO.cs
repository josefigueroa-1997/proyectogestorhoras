using Microsoft.AspNetCore.Routing.Constraints;

namespace Proyectogestionhoras.Models.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Nombre_Usuario { get; set; }
        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public string? NOMBRE_RECURSO_CORRELATIVO { get; set; }
        public string? RolRecurso { get; set; }
        public int Numero_Horas { get; set; }
        public decimal Costo_Unitario { get; set; }
        public string? Rol { get; set; }

        public decimal? HH_MENSUALES { get; set; }

        public int ID_RECURSO { get;set; }
        public decimal? HH_ANUALES { get; set; }
        public decimal? PROCENTAJE_PROYECTO { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string? Tipo_Consultor { get; set; }


    }
}
