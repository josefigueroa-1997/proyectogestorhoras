using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
namespace Proyectogestionhoras.Services.Interface
{
    public interface ICliente
    {
        public Task<List<Cliente>> ObtenerClientesIndex(int? id);
        
    }
}
