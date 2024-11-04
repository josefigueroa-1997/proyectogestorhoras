using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
namespace Proyectogestionhoras.Services.Interface
{
    public interface IUsuario
    {
        public Task<bool> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int? numhoras, decimal costounitario, float? porcentajehoras, DateTime? fechainicio, DateTime? fechafin);
        public Task<List<Rol>> ObtenerRoles();
        public Task<bool> EditarUsuario(int idusuario, string nombre, string nombreusuario, string telefono, string email, int? hhsemanales, decimal costo, float? porcentaje, DateTime? fechainicio, DateTime? fechatermino);
        public Task<List<UsuarioDTO>> ObtenerUusario(int? id,string? nombre,int? id_recurso);
        public Task<bool> Login(string rut, string contrasena);
        public Task<Login> ObtenerCredenciales(string rut);
        public Task<List<UsuarioProyectoDTO>> ObtenerUsuariosProyecto(int idusuario);
        public Task<List<HH_AsignaciónDTO>> RecuperarHHUsuarios(int idusuario, int idproyecto);
        public Task<List<StockHorasDTO>> ConsultaStockHoras(int idusuario);

        public Task<bool> CambiarContrasena(Usuario usuario);

    }
}
