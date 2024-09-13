using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IProyecto
    {
        public Task<List<ProyectoDTO>> ObtenerProyectos(int? id,int? idcliente,int? idusuario,string? nombre,int? idtipoempresa,string? statusproyecto);
    }
}
