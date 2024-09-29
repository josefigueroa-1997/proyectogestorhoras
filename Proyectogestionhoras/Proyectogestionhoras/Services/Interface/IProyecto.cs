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
        public Task<List<ServiciosDTO>> ObtenerValoresServicios(int idcodigo);
        public Task<List<Segmento>> GetValoresSegmentos(int idcodigo);
        public Task<List<Cuentum>> GetValoresGastos(int idsegmento);
        public Task<List<Gasto>> ObtenerGastos();
        public Task<List<Servicio>> ObtenerServicios();
        public Task<List<ConsultoresDTO>> ObtenerValoresConsultores(int idcodigo);
        public Task<List<HonorariosDTO>> ObtenerValoresHonorarios(int idcodigo,int idrecurso);
    }
}
