using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IPlanilla
    {
        public Task<int> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones,int idactividad);
        public Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaUsuario(int? idusuario,int? idplanilla);


    }
}
