using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
namespace Proyectogestionhoras.Services.Interface
{
    public interface ICliente
    {
        public Task<List<ClienteDTO>> ObtenerClientesIndex(int? id);
        public Task<bool> RegistrarCliente(string nombre,string direccion,string ciudad,string pais,string telefono,string? pagweb,string? linkedin,string? instagram,string idcliente,string sucursal);
        public Task<bool> EditarCliente(int id, string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram,int idsucursal,string nombresucursal);
        public Task<bool> EliminarCliente(int id);
        public Task<bool> AgregarSucursal(int idsucursal, string nombre);
        public Task <List<Sucursal>> ObtenerSucursal(int idcliente);
        public Task<int> ValidarExistenciaIdCliente(string idcliente);
        //public Task<List<Cliente>> GetClientes(int? id,string? nombre, string? pais);
    }
}
