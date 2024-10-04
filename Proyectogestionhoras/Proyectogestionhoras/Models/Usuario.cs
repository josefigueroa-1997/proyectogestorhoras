using System;
using System.Collections.Generic;

namespace Proyectogestionhoras.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            HistorialCostos = new HashSet<HistorialCosto>();
            UsuarioProyectos = new HashSet<UsuarioProyecto>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string? Contrasena { get; set; }
        public int IdRol { get; set; }
        public int IdRecurso { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public virtual Recurso IdRecursoNavigation { get; set; } = null!;
        public virtual Rol IdRolNavigation { get; set; } = null!;
        public virtual ICollection<HistorialCosto> HistorialCostos { get; set; }
        public virtual ICollection<UsuarioProyecto> UsuarioProyectos { get; set; }
    }
}
