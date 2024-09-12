using Proyectogestionhoras.Models;
namespace Proyectogestionhoras.Services.Interface
{
    public interface ICliente
    {
        public Task<List<Cliente>> ObtenerClientesIndex();
    }
}
