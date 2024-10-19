using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IReporte
    {
        public Task<List<RepoteFacturasDTO>> RecuperarFacturasProyectos();
    }
}
