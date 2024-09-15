using Microsoft.EntityFrameworkCore.Storage;
using Proyectogestionhoras.Models;
namespace Proyectogestionhoras.Services.Interface
{
    public interface IContacto
    {
        public Task<bool> RegistrarContacto(string nombre,string cargo,DateOnly desde,DateOnly hasta,string email,string telefono,int idcliente);
        public Task<List<Contacto>> ObtenerContactos(int? id,int? idcliente, string? nombre);
        public Task<bool> EditarContacto(int id,string nombre, string cargo, DateOnly desde, DateOnly hasta, string email, string telefono, int idcliente);
        public Task<bool> EliminarContacto(int id);

    }
}
