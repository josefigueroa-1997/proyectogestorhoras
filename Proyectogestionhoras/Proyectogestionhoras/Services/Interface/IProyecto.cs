using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IProyecto
    {
        public Task<List<Unegocio>> ObtenerUnegocio();
        public Task<List<Ccosto>> ObtenerCcosto();
        public Task<List<CcostoUnegocio>> ObtenerCodigoCCosto(int idcosto,int idunegocio);
        public Task<List<Empresa>> ObtenerEmpresa();
        public Task<List<Tipologium>> ObtenerTipoligias();
        public Task<List<StatusProyecto>> ObtenerStatus(int id);
        public Task<List<ProyectoDTO>> ObtenerProyectos(int? id,int? idcliente,int? idusuario,string? nombre,int? idtipoempresa,string? statusproyecto);
        public Task<List<FacturaDTO>> ObtenerValoresFactura(int idcosto, int idunegocio);
    }
}
