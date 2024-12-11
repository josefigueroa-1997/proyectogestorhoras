using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IPlanilla
    {
        public Task<int> RegistrarHoras(int idusuario, int idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones,int idactividad);
        public Task<int> RegistrarHorasEmpresa(int idusuario, string horasasignadas, DateTime Fecharegistro, string? observaciones,int idsubactividad);
        public Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaUsuario(int? idusuario,int? idplanilla);

        public Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaExcel(int idplanilla);
    }
}
