using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;

namespace Proyectogestionhoras.Services
{
    public class ReporteService : IReporte
    {
        private readonly Conexion conexion;

        public ReporteService(Conexion conexion)
        {
            this.conexion = conexion;
        }
        public async Task<List<RepoteFacturasDTO>> RecuperarFacturasProyectos()
        {

            try
            {


                var facturas = new List<RepoteFacturasDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEFACTURAS";
                    command.CommandType = CommandType.StoredProcedure;
               
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RepoteFacturasDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                TipoStatus = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                Neto = reader.IsDBNull(reader.GetOrdinal("Neto")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Neto")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                            };
                            facturas.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return facturas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las facturas de los proyectos:" + ex.Message);
                return new List<RepoteFacturasDTO>();

            }
        }

        public async Task<List<ReporteHHAsignadasDTO>> RecuperarHHProyectos(string recurso)
        {
            try
            {


                var hhasignadas = new List<ReporteHHAsignadasDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEHHSOCIOSSTAFF";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRERECURSO", recurso));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteHHAsignadasDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                TipoStatus = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                HHRecurso = reader.IsDBNull(reader.GetOrdinal("HH_Recurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HH_Recurso")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                            };
                            hhasignadas.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return hhasignadas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las hh asignadas de los proyectos:" + ex.Message);
                return new List<ReporteHHAsignadasDTO>();

            }
        }

        public async Task<List<ReporteHHAsignadasDTO>> RecuperarHHConsultoresProyectos(string tipoconsultor)
        {
            try
            {


                var hhasignadas = new List<ReporteHHAsignadasDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEHHCONSULTORES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@TIPOCONSULTOR", tipoconsultor));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteHHAsignadasDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                TipoStatus = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                HHRecurso = reader.IsDBNull(reader.GetOrdinal("HH_Recurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HH_Recurso")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                            };
                            hhasignadas.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return hhasignadas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las hh asignadas de los proyectos:" + ex.Message);
                return new List<ReporteHHAsignadasDTO>();

            }
        }
        public async Task<List<ReportesCostoRecursosDTO>> RecuperarCostosProyectos(string recurso)
        {
            try
            {


                var costos = new List<ReportesCostoRecursosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTECOSTOSOCIOSSTAFF";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRERECURSO", recurso));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReportesCostoRecursosDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                TipoStatus = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                CostoRecurso = reader.IsDBNull(reader.GetOrdinal("COSTO_Recurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_Recurso")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                            };
                            costos.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return costos;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los costos asignados de los proyectos:" + ex.Message);
                return new List<ReportesCostoRecursosDTO>();

            }
        }

        public async Task<List<ReportesCostoRecursosDTO>> RecuperarCostosConsultoresProyectos(string tipoconsultor)
        {
            try
            {


                var costos = new List<ReportesCostoRecursosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTECOSTOCONSULTORES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@TIPOCONSULTOR", tipoconsultor));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReportesCostoRecursosDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                TipoStatus = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                CostoRecurso = reader.IsDBNull(reader.GetOrdinal("COSTO_Recurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_Recurso")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                            };
                            costos.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return costos;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los costos asignados de los proyectos:" + ex.Message);
                return new List<ReportesCostoRecursosDTO>();

            }
        }
        public async Task<List<ReporteMargenProyectoDTO>> RecuperarMargenProyectos()
        {
            try
            {


                var margenes = new List<ReporteMargenProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEMARGEN";
                    command.CommandType = CommandType.StoredProcedure;
                 
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteMargenProyectoDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUM_PROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBRE_PROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRE_PROYECTO")),
                                Monto = reader.IsDBNull(reader.GetOrdinal("MONTO_PROYECTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTO_PROYECTO")),
                                TotalGastos = reader.IsDBNull(reader.GetOrdinal("TOTAL_GASTOS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TOTAL_GASTOS")),
                                TotalServicios = reader.IsDBNull(reader.GetOrdinal("TOTAL_SERVICIOS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TOTAL_SERVICIOS")),
                                CostosSocios = reader.IsDBNull(reader.GetOrdinal("COSTO_SOCIOS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_SOCIOS")),
                                CostosStaff = reader.IsDBNull(reader.GetOrdinal("COSTO_STAFF")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_STAFF")),
                                CostoConsultorA = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORA")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORA")),
                                CostoConsultorB = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORB")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORB")),
                                CostoConsultorC = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORC")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORC")),
                                Margen = reader.IsDBNull(reader.GetOrdinal("MARGEN_DE_CONTRIBUCION")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MARGEN_DE_CONTRIBUCION")),
                            };
                            margenes.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return margenes;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los margenes  de los proyectos:" + ex.Message);
                return new List<ReporteMargenProyectoDTO>();

            }
        }
    }
}
