using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
namespace Proyectogestionhoras.Services.Interface
{
    public interface IUsuario
    {
        public Task<bool> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int? numhoras, decimal costounitario, float? porcentajehoras, DateTime? fechainicio, DateTime? fechafin);
        public Task<List<Rol>> ObtenerRoles();
       // public Task<List<SegmentoCosto>> ObtenerSegmentoCosto();
        public Task<List<UsuarioDTO>> ObtenerUusario(int? id,string? nombre,int? id_recurso);
        public Task<bool> Login(string rut, string contrasena);
        public Task<Login> ObtenerCredenciales(string rut);
        public Task<List<UsuarioProyectoDTO>> ObtenerHorasUsuariosProyecto(int? idproyecto);

    }
}
