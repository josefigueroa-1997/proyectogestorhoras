using Proyectogestionhoras.Models.ViewModel;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IEjecucion
    {
        public Task GestorIngresos(int idproyecto,List<IngresoViewModel> ingreso);
    }
}
