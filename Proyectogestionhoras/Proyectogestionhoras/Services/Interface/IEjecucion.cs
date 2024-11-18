using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IEjecucion
    {
        public Task GestorIngresos(int idproyecto,List<IngresoViewModel> ingreso);
        public Task GestorServiciosReales(int idproyecto,List<ServiciosRealesViewModel> servicio);
        public Task GestorGastosReales(int idproyecto, List<GastosRealesViewModel> gasto);
        public Task<List<GastosRealesDTO>> ObtenerGastosReales(int? idproyecto);
    }
}
