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
    }
}
