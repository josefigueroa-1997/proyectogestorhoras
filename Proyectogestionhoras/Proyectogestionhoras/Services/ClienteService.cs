
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using Proyectogestionhoras.Models.DTO;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Proyectogestionhoras.Services
{
    public class ClienteService : ICliente
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public ClienteService(Conexion conexion, PROYECTO_CONTROL_HORASContext context)
        {
            this.conexion = conexion;
            this.context = context;
        }
        public async Task<List<ClienteDTO>> ObtenerClientesIndex(int? id)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                #pragma warning restore CS8600
                var clientes = new List<ClienteDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCLIENTESINDEX";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCLIENTES",idparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) {
                            ClienteDTO cliente = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Direccion = reader.GetString(reader.GetOrdinal("DIRECCION")),
                                Ciudad = reader.GetString(reader.GetOrdinal("CIUDAD")),
                                Pais = reader.GetString(reader.GetOrdinal("PAIS")),
                                Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                PagWeb = reader.IsDBNull(reader.GetOrdinal("PAGWEB")) ? string.Empty : reader.GetString(reader.GetOrdinal("PAGWEB")),
                                Linkedin = reader.IsDBNull(reader.GetOrdinal("LINKEDIN")) ? string.Empty : reader.GetString(reader.GetOrdinal("LINKEDIN")),
                                Instagram = reader.IsDBNull(reader.GetOrdinal("INSTAGRAM")) ? string.Empty : reader.GetString(reader.GetOrdinal("INSTAGRAM")),
                                Id_Cliente = reader.GetString(reader.GetOrdinal("ID_CLIENTE")),
                                Sucursales_Cliente = reader.IsDBNull(reader.GetOrdinal("SUCURSALES_CLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("SUCURSALES_CLIENTE")),
                                SucursalIds = new List<int>()

                            };
                            clientes.Add(cliente);
                            
                        }
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int sucursalId = reader.GetInt32(reader.GetOrdinal("SucursalIds"));
                                clientes.Last().SucursalIds?.Add(sucursalId);
                            }
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return clientes;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<ClienteDTO>();

            }

        }
        public async Task<bool> RegistrarCliente(string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram,int idcliente,string sucursal)
        {
            try
            {
                #pragma warning disable CS8600
                object pagwebparameter = (object)pagweb ?? DBNull.Value;
                object linkedinparamater = (object)linkedin ?? DBNull.Value;
                object instagramparameter = (object)instagram ?? DBNull.Value;
                #pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REGISTRARCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@DIRECCION", direccion));
                    command.Parameters.Add(new SqlParameter("@CIUDAD", ciudad));
                    command.Parameters.Add(new SqlParameter("@PAIS", pais));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@PAG_WEB", pagwebparameter));
                    command.Parameters.Add(new SqlParameter("@LINKEDIN", linkedinparamater));
                    command.Parameters.Add(new SqlParameter("@INSTAGRAM", instagramparameter));
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE", idcliente));
                    command.Parameters.Add(new SqlParameter("@NOMBRE_SUCURSAL", sucursal));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex) {

                Debug.WriteLine($"Hubo un error al registrar el cliente"+ex);
                return false;
            }
        }
        public async Task<bool> EditarCliente(int id, string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram,int idsucursal,string nombresucursal)
        {
            try
            {
                #pragma warning disable CS8600
                object pagwebparameter = (object)pagweb ?? DBNull.Value;
                object linkedinparamater = (object)linkedin ?? DBNull.Value;
                object instagramparameter = (object)instagram ?? DBNull.Value;
                #pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ACTUALIZARCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID",id));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@DIRECCION", direccion));
                    command.Parameters.Add(new SqlParameter("@CIUDAD", ciudad));
                    command.Parameters.Add(new SqlParameter("@PAIS", pais));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@PAG_WEB", pagwebparameter));
                    command.Parameters.Add(new SqlParameter("@LINKEDIN", linkedinparamater));
                    command.Parameters.Add(new SqlParameter("@INSTAGRAM", instagramparameter));
                    command.Parameters.Add(new SqlParameter("@IDSUCURSAL", idsucursal));
                    command.Parameters.Add(new SqlParameter("@NOMBRESUCURSAL", nombresucursal));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Hubo un error al editar la información del cliente:"+ex.Message);
                return false;

            }
        }
        public async Task<bool> EliminarCliente(int id)
        {
            try
            {
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETECLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al eliminar al cliente: " + ex.Message);
                return false;
            }

        }

        public async Task<int> ValidarExistenciaIdCliente(int idcliente)
        {
            try 
            {
                int resultado = 2;
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "VERIFICARIDCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCLIENTE", idcliente));
                    resultado = (int)await command.ExecuteScalarAsync();
                }

                return resultado > 0 ? 1 : 2;

            }
            catch(Exception e)
            {
                Debug.WriteLine($"Hubo un error al validar la existencia del idcliente:"+e.Message);
                return 0;
            }
        }
        public async Task<bool> AgregarSucursal(int idsucursal, string nombre)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "AGREGAR_SUCURSAL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE", idsucursal));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al agregar una sucursal" + ex.Message);
                return false;

            }
        }
        public async Task<List<Sucursal>> ObtenerSucursal(int idcliente)
        {
            try
            {

                var sucursales = new List<Sucursal>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSUCURSALES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCLIENTE", idcliente));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Sucursal sucursal = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                               


                            };
                            sucursales.Add(sucursal);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return sucursales;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las sucursales:" + ex.Message);
                return new List<Sucursal>();

            }
        }
    }
}
