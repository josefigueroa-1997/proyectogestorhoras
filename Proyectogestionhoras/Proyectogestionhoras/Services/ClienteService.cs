
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
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
        public async Task<List<Cliente>> ObtenerClientesIndex(int? id)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                #pragma warning restore CS8600
                var clientes = new List<Cliente>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCLIENTESINDEX";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCLIENTES",idparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) {
                            Cliente cliente = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                Direccion = reader.GetString(reader.GetOrdinal("DIRECCION")),
                                Ciudad = reader.GetString(reader.GetOrdinal("CIUDAD")),
                                Pais = reader.GetString(reader.GetOrdinal("PAIS")),
                                Telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                PagWeb = reader.GetString(reader.GetOrdinal("PAGWEB")),
                                Linkedin = reader.GetString(reader.GetOrdinal("LINKEDIN")),
                                Instagram = reader.GetString(reader.GetOrdinal("INSTAGRAM")),


                            };
                            clientes.Add(cliente);
                            Debug.WriteLine(cliente.Ciudad);
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return clientes;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<Cliente>();

            }

        }   
        
    }
}
