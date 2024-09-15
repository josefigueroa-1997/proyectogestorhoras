using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
namespace Proyectogestionhoras.Services.Interface
{
    public interface ICliente
    {
        public Task<List<Cliente>> ObtenerClientesIndex(int? id);
        public Task<bool> RegistrarCliente(string nombre,string direccion,string ciudad,string pais,string telefono,string? pagweb,string? linkedin,string? instagram,string idcliente);
        public Task<bool> EditarCliente(int id, string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram);
        public Task<bool> EliminarCliente(int id);
        //public Task<List<Cliente>> GetClientes(int? id,string? nombre, string? pais);
    }
}
