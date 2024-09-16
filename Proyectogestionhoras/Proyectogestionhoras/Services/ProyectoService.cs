using Proyectogestionhoras.Services.Interface;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;
namespace Proyectogestionhoras.Services
{
    public class ProyectoService : IProyecto
    {
        private readonly Conexion conexion;
        public ProyectoService(Conexion conexion) { 
            
            this.conexion = conexion;
        }

        public async Task<List<ProyectoDTO>> ObtenerProyectos(int? id, int? idcliente, int? idusuario, string? nombre, int? idtipoempresa, string? statusproyecto)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                object idclienteparameter = (object)idcliente ?? DBNull.Value;
                object idusuarioparameter = (object)idusuario ?? DBNull.Value;
                object nombreparameter = (object)nombre ?? DBNull.Value;
                object idtipoempresaparameter = (object)idtipoempresa ?? DBNull.Value;
                object statusproyectoparameter = (object)statusproyecto ?? DBNull.Value;
                #pragma warning restore CS8600
                var proyectos = new List<ProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using(DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "OBTENERPROYECTOS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ID",idparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDCLIENTE", idclienteparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
                    cmd.Parameters.Add(new SqlParameter("@NOMBRE", nombreparameter));
                    cmd.Parameters.Add(new SqlParameter("@ID_TIPOEMPRESA", idtipoempresaparameter));
                    cmd.Parameters.Add(new SqlParameter("@STATUS_PROYECTO", statusproyectoparameter));
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProyectoDTO proyecto = new() { 
                            
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                NombreProyecto = reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                num_proyecto = reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                Id_Cliente = reader.GetInt32(reader.GetOrdinal("ID_CLIENTE")),
                                Tipo_Status = reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                Fecha_Inicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")).Date,
                                Fecha_Termino = reader.GetDateTime(reader.GetOrdinal("FECHA_TERMINO")).Date ,
                                Plazo = reader.GetInt32(reader.GetOrdinal("PLAZO")),
                                Nombre_Cliente = reader.GetString(reader.GetOrdinal("NOMBRE_CLIENTE")),
                                Tipo_Tipologia = reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                Tipo_Empresa = reader.GetString(reader.GetOrdinal("TIPO_EMPRESA")),
                                CCOSTO_NEGOCIO = reader.GetString(reader.GetOrdinal("CCOSTO_NEGOCIO")),
                                Linkedin = reader.IsDBNull(reader.GetOrdinal("LINKEDIN")) ? string.Empty : reader.GetString(reader.GetOrdinal("LINKEDIN")),
                                Integrantes_Proyecto = reader.GetString(reader.GetOrdinal("INTEGRANTES_PROYECTO")),
                                HH_Semanales = reader.GetInt32(reader.GetOrdinal("HH_SEMANALES")),
                                Nombre_Recurso = reader.GetString(reader.GetOrdinal("NOMBRE_RECURSO")),
                                Pais = reader.GetString(reader.GetOrdinal("PAIS")),
                                Ciudad = reader.GetString(reader.GetOrdinal("CIUDAD"))
                                
                            };
                            proyectos.Add(proyecto);
                            
                        }
                    }
                    await conexion.CloseDatabaseConnectionAsync();
                    return proyectos;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los proyectos" + ex);
                return new List<ProyectoDTO>();

            }
        }
        public async Task<List<Unegocio>> ObtenerUnegocio()
        {
            try
            {
                var unegocios = new List<Unegocio>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERUNEGOCIO";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Unegocio unegocio = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoUnegocio = reader.GetString(reader.GetOrdinal("TIPO_UNEGOCIO")),

                            };
                            unegocios.Add(unegocio);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return unegocios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los unegocios:" + ex.Message);
                return new List<Unegocio>();
            }
        }
        public async Task<List<Ccosto>> ObtenerCcosto()
        {
            try
            {
                var ccostos = new List<Ccosto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCCOSTO";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Ccosto ccosto = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoCcosto = reader.GetString(reader.GetOrdinal("TIPO_CCOSTO")),

                            };
                            ccostos.Add(ccosto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return ccostos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los ccostos:" + ex.Message);
                return new List<Ccosto>();
            }
        }
        public async Task<List<Empresa>> ObtenerEmpresa()
        {
            try
            {
                var empresas = new List<Empresa>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERTIPOEMPRESA";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Empresa empresa = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoEmpresa = reader.GetString(reader.GetOrdinal("TIPO_EMPRESA")),

                            };
                            empresas.Add(empresa);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return empresas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las empresas:" + ex.Message);
                return new List<Empresa>();
            }
        }
        public async Task<List<Tipologium>> ObtenerTipoligias()
        {
            try
            {
                var tipologias = new List<Tipologium>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERTIPOLOGIA";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Tipologium tipologia = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoTipologia = reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),

                            };
                            tipologias.Add(tipologia);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return tipologias;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Tipologium>();
            }
        }
    }
}
