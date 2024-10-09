using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
namespace Proyectogestionhoras.Services.Interface
{
    public interface IProyecto
    {
        public Task<bool> CrearProyecto(decimal monto,string moneda,string afectaiva,int idtipologia,string nombre,string numproyecto,DateTime fechainicio,DateTime fechatermino,int plazo,int tipoempresa,int codigoccosto,int idclientesucursal,int status,string? probabilidad,decimal? porcentajeprobabilidad,DateTime? fechaplazoneg,int hhsocios,int hhstaff,int hhconsultora,int hhconsultorb,int hhconsultorc,int idsegmentosocio,int idsegmentostaff,int idsegmentoconsultora,int idsegmentoconsultorb,int idsegmentoconsultorc,int idsegmentofactura,List<ServicioViewModel> servicios,List<GastoViewModel> gastos);

        public Task<bool> EditarProyecto(int idproyecto,int idpresupuesto,decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, List<ServicioViewModel> servicios, List<GastoViewModel> gastos);

        public Task<List<Unegocio>> ObtenerUnegocio();
        public Task<List<Ccosto>> ObtenerCcosto();
        public Task<List<CcostoUnegocio>> ObtenerCodigoCCosto(int idcosto,int idunegocio);
        public Task<List<Empresa>> ObtenerEmpresa();
        public Task<List<Tipologium>> ObtenerTipoligias();
        public Task<List<StatusProyecto>> ObtenerStatus();
        public Task<List<ProyectoDTO>> ObtenerProyectos(int? id,int? idcliente,string? nombre,int? idtipoempresa,int? statusproyecto,string? numproyecto,int? idtipologia,int? unidadneg,int? idccosto);
        public Task<List<FacturaDTO>> ObtenerValoresFactura(int idcosto, int idunegocio);
        public Task<List<ServiciosDTO>> ObtenerValoresServicios(int idcosto,int idunegocio);
        public Task<List<Segmento>> GetValoresSegmentos(int idcodigo);
        
        public Task<List<Gasto>> ObtenerGastos();
        public Task<List<Servicio>> ObtenerServicios();
        public Task<List<ConsultoresDTO>> ObtenerValoresConsultores(int idcosto, int idunegocio);
        public Task<List<HonorariosDTO>> ObtenerValoresHonorarios(int idcosto,int idunegocio,int idrecurso);
        public Task<List<GastoDTO>> ObtenerValoresGastos(int idcodigo, string nombregasto);
        public Task<List<ServiciosProyectoDTO>> ObtenerServiciosProyecto(int? idproyecto);
        public Task<List<GastoProyectoDTO>> ObtenerGastosProyectos(int? idproyecto);
    }
}
