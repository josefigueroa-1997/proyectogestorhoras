using Proyectogestionhoras.Models.ViewModel;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IExcel
    {
        public Task IngresarEgresosMasivosExcel(List<EgresosExcelViewModel> egresos);
    }
}
