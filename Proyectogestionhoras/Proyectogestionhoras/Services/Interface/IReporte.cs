using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IReporte
    {
        public Task<List<RepoteFacturasDTO>> RecuperarFacturasProyectos();
        public Task<List<ReporteHHAsignadasDTO>> RecuperarHHProyectos(string recurso);
        public Task<List<ReporteHHAsignadasDTO>> RecuperarHHConsultoresProyectos(string tipoconsultor);
        public Task<List<ReportesCostoRecursosDTO>> RecuperarCostosProyectos(string recurso);
        public Task<List<ReportesCostoRecursosDTO>> RecuperarCostosConsultoresProyectos(string tipoconsultor);
        public Task<List<ReporteMargenProyectoDTO>> RecuperarMargenProyectos();
        public Task<List<ReporteControlHHDTO>> ReporteControlHH(int idproyecto);
        public Task<List<HorasSociosDTO>> ObtenerHorasPorSocio(int mes);
        public Task<List<ReporteNegociacionDTO>> ReporteProyectoNegociacion();
        public Task<List<ReportQuarterDTO>> RecuperarQuarter(int anio);
        public Task<List<ReporteNegociacionKeyDTO>> ReporteNegociacionKeys();
        public Task<List<BasedeDatosDTO>> ExportarBasedeDatos();
        public Task<List<ReporteBalanceDTO>> RecuperarBalanceHoras();
        public Task<List<ResumenCostosProyectoDTO>> ObtenerResumenCostosProyecto();
    }
}
