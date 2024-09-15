using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
namespace Proyectogestionhoras.Services
{
    public class ContactoService : IContacto
    {
        private readonly Conexion conexion;
        public ContactoService(Conexion conexion)
        {
            this.conexion = conexion;
        }

        public async Task<bool> RegistrarContacto(string nombre, string cargo, DateOnly desde, DateOnly hasta, string email, string telefono, int idcliente)
        {
            try
            {
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REGISTRARCONTACTOCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@CARGO", cargo));
                    command.Parameters.Add(new SqlParameter("@DESDE", desde.ToDateTime(TimeOnly.MinValue)));
                    command.Parameters.Add(new SqlParameter("@HASTA", hasta.ToDateTime(TimeOnly.MinValue)));
                    command.Parameters.Add(new SqlParameter("@EMAIL", email));
                    command.Parameters.Add(new SqlParameter("@TELFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE", idcliente));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar el contacto: " + ex.Message);
                return false;
            }
        }
        public async Task<List<Contacto>> ObtenerContactos(int? id, int? idcliente, string? nombre)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                object idclienteparameter = (object)idcliente ?? DBNull.Value;
                object nombreparameter = (object)nombre ?? DBNull.Value;
                #pragma warning restore CS8600
                var contactos = new List<Contacto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCONTACTOCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", idparameter));
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE",idclienteparameter));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombreparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Contacto contacto = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Cargo = reader.GetString(reader.GetOrdinal("CARGO")),
                                Desde = reader.GetDateTime(reader.GetOrdinal("DESDE")),
                                Hasta = reader.GetDateTime(reader.GetOrdinal("HASTA")),
                                Email = reader.GetString(reader.GetOrdinal("EMAIL")),
                                Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                IdCliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE"))


                            };
                            contactos.Add(contacto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return contactos;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<Contacto>();

            }
        }
        public async Task<bool> EditarContacto(int id, string nombre, string cargo, DateOnly desde, DateOnly hasta, string email, string telefono, int idcliente)
        {
            try
            {
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ACTUALIZARCONTACTOCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@CARGO", cargo));
                    command.Parameters.Add(new SqlParameter("@DESDE", desde.ToDateTime(TimeOnly.MinValue)));
                    command.Parameters.Add(new SqlParameter("@HASTA", hasta.ToDateTime(TimeOnly.MinValue)));
                    command.Parameters.Add(new SqlParameter("@EMAIL", email));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE", idcliente));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al actualizar el contacto: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> EliminarContacto(int id)
        {

            try
            {
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETECONTACTOCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al eliminar al contacto: " + ex.Message);
                return false;
            }
        }

    }
}
