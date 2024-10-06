using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IFactura
    {
        public Task<List<FacturaProyectoDTO>> RecuperarFacturas(int? idproyecto);
    }
}
