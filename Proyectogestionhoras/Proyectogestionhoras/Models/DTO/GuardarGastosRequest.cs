using Proyectogestionhoras.Models.ViewModel;

namespace Proyectogestionhoras.Models.DTO
{
    public class GuardarGastosRequest
    {
        public int idproyecto { get; set; }
        public List<GastosRealesViewModel>? gastos { get; set; }
    }
}
