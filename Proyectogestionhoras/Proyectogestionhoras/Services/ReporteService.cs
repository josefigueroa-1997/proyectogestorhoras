using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using iText.Bouncycastle.Crypto;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Services
{
    public class ReporteService : IReporte
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;

        public ReporteService(Conexion conexion, PROYECTO_CONTROL_HORASContext context)
        {
            this.conexion = conexion;
            this.context = context;
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
        public async Task<List<ReporteControlHHDTO>> ReporteControlHH(int idproyecto)
        {
            try
            {
                var controlhh = new List<ReporteControlHHDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerHorasPoryejecutadasProyecto";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ProyectoId", idproyecto));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteControlHHDTO datos = new()
                            {
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                                HorasSociosEjecutadas = reader.IsDBNull(reader.GetOrdinal("HorasSociosEjecutadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasSociosEjecutadas")),
                                HorasStaffEjecutadas = reader.IsDBNull(reader.GetOrdinal("HorasStaffEjecutadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasStaffEjecutadas")),
                                HorasConsultorAEjecutadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorAEjecutadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorAEjecutadas")),
                                HorasConsultorBEjecutadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorBEjecutadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorBEjecutadas")),
                                HorasConsultorCEjecutadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorCEjecutadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorCEjecutadas")),
                                HorasSociosPlaneadas = reader.IsDBNull(reader.GetOrdinal("HorasSociosPlaneadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasSociosPlaneadas")),
                                HorasStaffPlaneadas = reader.IsDBNull(reader.GetOrdinal("HorasStaffPlaneadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasStaffPlaneadas")),
                                HorasConsultorAPlaneadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorAPlaneadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorAPlaneadas")),
                                HorasConsultorBPlaneadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorBPlaneadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorBPlaneadas")),
                                HorasConsultorCPlaneadas = reader.IsDBNull(reader.GetOrdinal("HorasConsultorCPlaneadas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HorasConsultorCPlaneadas")),
                            };

                            controlhh.Add(datos);
                        }
                    }
                }

                await conexion.CloseDatabaseConnectionAsync();
                return controlhh;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener el control hh asignados de los proyectos: {ex.Message}");
                return new List<ReporteControlHHDTO>();
            }
        }
        public async Task<List<HorasSociosActDTO>> HorasPorSociosMesActual(int mes)
        {
            try
            {
                var horas = new List<HorasSociosActDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "sp_HorasSociosPorActividadMESACTUAL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var sociosDict = new Dictionary<string, HorasSociosActDTO>();

                        while (await reader.ReadAsync())
                        {
                            string nombreSocio = reader["NombreUsuario"] != DBNull.Value ? reader["NombreUsuario"].ToString() : "Sin nombre";
                            string actividad = reader["Actividad"] != DBNull.Value ? reader["Actividad"].ToString() : "Sin actividad";
                            decimal horasTotales = reader["HorasTotalesMesActual"] != DBNull.Value ? Convert.ToDecimal(reader["HorasTotalesMesActual"]) : 0;

                            // Si el socio ya está en el diccionario, solo agrega la actividad
                            if (!sociosDict.ContainsKey(nombreSocio))
                            {
                                sociosDict[nombreSocio] = new HorasSociosActDTO
                                {
                                    NombreSocio = nombreSocio
                                };
                            }

                            // Agregar actividad y horas a ese socio
                            sociosDict[nombreSocio].HorasPorActividad[actividad] = horasTotales;
                        }

                        horas = sociosDict.Values.ToList();
                    }
                }

                await conexion.CloseDatabaseConnectionAsync();
                return horas;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las horas por socios en el mes de los proyectos: {ex.Message}");
                return new List<HorasSociosActDTO>();
            }

        }

        public async Task<List<HorasSociosActDTO>> HorasPorSociosAnioActual(int mes)
        {
            try
            {
                var horas = new List<HorasSociosActDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "sp_HorasSociosPorActividadANIOACTUAL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var sociosDict = new Dictionary<string, HorasSociosActDTO>();

                        while (await reader.ReadAsync())
                        {
                            string nombreSocio = reader["NombreUsuario"] != DBNull.Value ? reader["NombreUsuario"].ToString() : "Sin nombre";
                            string actividad = reader["Actividad"] != DBNull.Value ? reader["Actividad"].ToString() : "Sin actividad";
                            decimal horasTotales = reader["HorasTotalesMesActual"] != DBNull.Value ? Convert.ToDecimal(reader["HorasTotalesMesActual"]) : 0;

                            // Si el socio ya está en el diccionario, solo agrega la actividad
                            if (!sociosDict.ContainsKey(nombreSocio))
                            {
                                sociosDict[nombreSocio] = new HorasSociosActDTO
                                {
                                    NombreSocio = nombreSocio
                                };
                            }

                            // Agregar actividad y horas a ese socio
                            sociosDict[nombreSocio].HorasPorActividad[actividad] = horasTotales;
                        }

                        horas = sociosDict.Values.ToList();
                    }
                }

                await conexion.CloseDatabaseConnectionAsync();
                return horas;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las horas por socios en el mes de los proyectos: {ex.Message}");
                return new List<HorasSociosActDTO>();
            }

        }
        public async Task<List<HorasSociosActDTO>> HorasPorSociosUltimosDoceMeses(int mes)
        {
            try
            {
                var horas = new List<HorasSociosActDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "sp_HorasSociosPorActividad12meses";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var sociosDict = new Dictionary<string, HorasSociosActDTO>();

                        while (await reader.ReadAsync())
                        {
                            string nombreSocio = reader["NombreUsuario"] != DBNull.Value ? reader["NombreUsuario"].ToString() : "Sin nombre";
                            string actividad = reader["Actividad"] != DBNull.Value ? reader["Actividad"].ToString() : "Sin actividad";
                            decimal horasTotales = reader["HorasTotalesMesActual"] != DBNull.Value ? Convert.ToDecimal(reader["HorasTotalesMesActual"]) : 0;

                            // Si el socio ya está en el diccionario, solo agrega la actividad
                            if (!sociosDict.ContainsKey(nombreSocio))
                            {
                                sociosDict[nombreSocio] = new HorasSociosActDTO
                                {
                                    NombreSocio = nombreSocio
                                };
                            }

                            // Agregar actividad y horas a ese socio
                            sociosDict[nombreSocio].HorasPorActividad[actividad] = horasTotales;
                        }

                        horas = sociosDict.Values.ToList();
                    }
                }

                await conexion.CloseDatabaseConnectionAsync();
                return horas;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las horas por socios en el mes de los proyectos: {ex.Message}");
                return new List<HorasSociosActDTO>();
            }

        }
        public async Task<List<ReporteNegociacionDTO>> ReporteProyectoNegociacion()
        {
            try
            {


                var facturas = new List<ReporteNegociacionDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTENEOGOCIACION";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteNegociacionDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                Probabilidad = reader.IsDBNull(reader.GetOrdinal("Probabilidad")) ? string.Empty : reader.GetString(reader.GetOrdinal("Probabilidad")),
                                Neto = reader.IsDBNull(reader.GetOrdinal("Neto")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Neto")),
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                TipoTipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                                Unegocio = reader.IsDBNull(reader.GetOrdinal("Unegocio")) ? string.Empty : reader.GetString(reader.GetOrdinal("Unegocio")),
                                CCosto = reader.IsDBNull(reader.GetOrdinal("CCosto")) ? string.Empty : reader.GetString(reader.GetOrdinal("CCosto")),
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

                Debug.WriteLine($"Hubo un error al obtener las facturas de los proyectos en negociación:" + ex.Message);
                return new List<ReporteNegociacionDTO>();

            }
        }

      
        public async Task<List<ReporteNegociacionKeyDTO>> ReporteNegociacionKeys()
        {
            try
            {


                var negociacion = new List<ReporteNegociacionKeyDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERNEGOCIACIONKEYIND";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteNegociacionKeyDTO datos = new()
                            {
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                HHSocios = reader.IsDBNull(reader.GetOrdinal("HH_SOCIOS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_SOCIOS")),
                                Monto = reader.IsDBNull(reader.GetOrdinal("MONTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                Probabilidad = reader.IsDBNull(reader.GetOrdinal("PROBABILIDAD")) ? string.Empty : reader.GetString(reader.GetOrdinal("PROBABILIDAD")),
                                FechaPlazo = reader.IsDBNull(reader.GetOrdinal("FECHA_PLAZO_NEG")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FECHA_PLAZO_NEG")),
                                FechaInicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")),
                                FechaTermino = reader.GetDateTime(reader.GetOrdinal("FECHA_TERMINO")),
                                Tipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                Codigo = reader.IsDBNull(reader.GetOrdinal("CODIGO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CODIGO")),
                                Plazo = reader.IsDBNull(reader.GetOrdinal("PLAZO")) ? 0 : reader.GetInt32(reader.GetOrdinal("PLAZO")),

                            };
                            negociacion.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return negociacion;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los proyectos en negociacion en key:" + ex.Message);
                return new List<ReporteNegociacionKeyDTO>();

            }
        }

        public async Task<List<CuotasMensualesDTO>> recuperarcuotasmensuales(int mes,int anio)
        {
            try
            {


                var cuotas = new List<CuotasMensualesDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerCuotasMensuales";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));
                    command.Parameters.Add(new SqlParameter("@Ano", anio));
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CuotasMensualesDTO datos = new()
                            {
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Ano")) ? 0 : reader.GetInt32(reader.GetOrdinal("Ano")),
                                Monto = reader.IsDBNull(reader.GetOrdinal("MontoTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                                
                            };
                            cuotas.Add(datos);
                            
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return cuotas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las cuotas mensuales:" + ex.Message);
                return new List<CuotasMensualesDTO>();

            }
        }

        public  List<QuarterData> ObtenerMontosPorQuarter()
        {
            int anioActual = DateTime.Now.Year;
            int anioSiguiente = anioActual + 1;

            var datos = context.MetaFacturacionesqxes
                .Where(m => m.Anio == anioActual || m.Anio == anioSiguiente)
                .SelectMany(m => new List<QuarterData>
                {
            new QuarterData { Anio = m.Anio, Quarter = "Q1", Meses = "Enero, Febrero, Marzo", Monto = m.MontoQ1 ?? 0 },
            new QuarterData { Anio = m.Anio, Quarter = "Q2", Meses = "Abril, Mayo, Junio", Monto = m.MontoQ2 ?? 0 },
            new QuarterData { Anio = m.Anio, Quarter = "Q3", Meses = "Julio, Agosto, Septiembre", Monto = m.MontoQ3 ?? 0 },
            new QuarterData { Anio = m.Anio, Quarter = "Q4", Meses = "Octubre, Noviembre, Diciembre", Monto = m.MontoQ4 ?? 0 }
                })
                .ToList();

            return datos;
        }

        public async Task<List<CuotasMensualesDTO>> recuperarfacturamensual(int mes, int anio)
        {
            try
            {


                var cuotas = new List<CuotasMensualesDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GetMontosUltimosYProximosTresMeses";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));
                    command.Parameters.Add(new SqlParameter("@Anio", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CuotasMensualesDTO datos = new()
                            {
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                
                                Monto = reader.IsDBNull(reader.GetOrdinal("Monto")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Monto")),

                            };
                            cuotas.Add(datos);
                            Debug.WriteLine(datos.Mes);
                            Debug.WriteLine(datos.Monto);
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return cuotas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las cuotas mensuales:" + ex.Message);
                return new List<CuotasMensualesDTO>();

            }
        }
        public async Task<List<HorasKeyDTO>> recuperarhhposibles(int mes, int anio)
        {
            try
            {


                var horas = new List<HorasKeyDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerHorasPorMeses";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));
                    command.Parameters.Add(new SqlParameter("@Anio", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HorasKeyDTO datos = new()
                            {
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                                Horas = reader.IsDBNull(reader.GetOrdinal("Horas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Horas")),

                            };
                            horas.Add(datos);
            
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return horas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las horas mensuales:" + ex.Message);
                return new List<HorasKeyDTO>();

            }
        }
        public async Task<List<HorasKeyDTO>> recuperarhhproyectos(int mes, int anio)
        {
            try
            {


                var horas = new List<HorasKeyDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerHorasSocioPorMes";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Mes", mes));
                    command.Parameters.Add(new SqlParameter("@Anio", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HorasKeyDTO datos = new()
                            {
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                          
                                Horas = reader.IsDBNull(reader.GetOrdinal("TotalHoras")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalHoras")),

                            };
                            horas.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return horas;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las horas mensuales:" + ex.Message);
                return new List<HorasKeyDTO>();

            }
        }


        public async Task<List<ReportQuarterDTO>> RecuperarQuarter(int anio)
        {
            try
            {


                var ingresos = new List<ReportQuarterDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ObtenerResumenTrimestral";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@Anio", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReportQuarterDTO datos = new()
                            {
                                Tipo = reader.IsDBNull(reader.GetOrdinal("Tipo")) ? string.Empty : reader.GetString(reader.GetOrdinal("Tipo")),
                                MontoQ1 = reader.IsDBNull(reader.GetOrdinal("MontoQ1")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoQ1")),
                                MontoQ2 = reader.IsDBNull(reader.GetOrdinal("MontoQ2")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoQ2")),
                                MontoQ3 = reader.IsDBNull(reader.GetOrdinal("MontoQ3")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoQ3")),
                                MontoQ4 = reader.IsDBNull(reader.GetOrdinal("MontoQ4")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoQ4")),

                                

                            };
                            ingresos.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return ingresos;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el resumen de los quarters:" + ex.Message);
                return new List<ReportQuarterDTO>();

            }
        }

        public async Task<List<BasedeDatosDTO>> ExportarBasedeDatos()
        {
            try
            {

                var proyectos = new List<BasedeDatosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DESCARGARBASEDEDATOS";
                    cmd.CommandType = CommandType.StoredProcedure;
                  
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            BasedeDatosDTO proyecto = new()
                            {

                                Id = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUM_PROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                Unegocio = reader.IsDBNull(reader.GetOrdinal("TIPO_UNEGOCIO")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_UNEGOCIO")),
                                CCosto = reader.IsDBNull(reader.GetOrdinal("TIPO_CCOSTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_CCOSTO")),
                                Codigo = reader.IsDBNull(reader.GetOrdinal("CODIGO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CODIGO")),
                                Cliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                Tipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                Empresa = reader.IsDBNull(reader.GetOrdinal("TIPO_EMPRESA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_EMPRESA")),
                                AfectaIva = reader.IsDBNull(reader.GetOrdinal("AFECTAIVA")) ? string.Empty : reader.GetString(reader.GetOrdinal("AFECTAIVA")),
                                Monto = reader.IsDBNull(reader.GetOrdinal("MONTO")) ? 0m : reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                Moneda = reader.IsDBNull(reader.GetOrdinal("MONEDA")) ? string.Empty : reader.GetString(reader.GetOrdinal("MONEDA")),
                                Status = reader.IsDBNull(reader.GetOrdinal("TIPO_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                Probabilidad = reader.IsDBNull(reader.GetOrdinal("PROBABILIDAD")) ? string.Empty : reader.GetString(reader.GetOrdinal("PROBABILIDAD")),
                                Porcentaje = reader.IsDBNull(reader.GetOrdinal("PORCENTAJE_PROBABILIDAD")) ? 0m : reader.GetDecimal(reader.GetOrdinal("PORCENTAJE_PROBABILIDAD")),
                                Plazo = reader.IsDBNull(reader.GetOrdinal("PLAZO")) ? 0 : reader.GetInt32(reader.GetOrdinal("PLAZO")),
                                Fechainicio = reader.IsDBNull(reader.GetOrdinal("FECHA_INICIO")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")),
                                Fechatermino = reader.IsDBNull(reader.GetOrdinal("FECHA_TERMINO")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FECHA_TERMINO")),
                                FechaPlazo = reader.IsDBNull(reader.GetOrdinal("FECHA_PLAZO_NEG")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FECHA_PLAZO_NEG")),
                                Departamento = reader.IsDBNull(reader.GetOrdinal("NOMBREDEPARTAMENTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREDEPARTAMENTO")),
                                HHsocios = reader.IsDBNull(reader.GetOrdinal("HH_SOCIOS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_SOCIOS")),
                                HHstaff = reader.IsDBNull(reader.GetOrdinal("HH_STAFF")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_STAFF")),
                                HHConsultorA = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTORA")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTORA")),
                                HHConsultorB = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTORB")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTORB")),
                                HHConsultorC = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTORC")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTORC")),
                                Costosocios = reader.IsDBNull(reader.GetOrdinal("COSTOSOCIOS")) ? 0m : reader.GetDecimal(reader.GetOrdinal("COSTOSOCIOS")),
                                Costostaff = reader.IsDBNull(reader.GetOrdinal("COSTOSSTAFF")) ? 0m : reader.GetDecimal(reader.GetOrdinal("COSTOSSTAFF")),
                                CostoconsultorA = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORA")) ? 0m : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORA")),
                                CostoconsultorB = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORB")) ? 0m : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORB")),
                                CostoconsultorC = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORC")) ? 0m : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORC")),
                                TotalServicios = reader.IsDBNull(reader.GetOrdinal("TOTALSERVICIO")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TOTALSERVICIO")),
                                TotalGastos = reader.IsDBNull(reader.GetOrdinal("TOTALGASTOS")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TOTALGASTOS")),
                                


                            };

                            proyectos.Add(proyecto);
                            Debug.WriteLine(proyecto.CCosto);
                            Debug.WriteLine(proyecto.AfectaIva);
                        }
                    }
                    await conexion.CloseDatabaseConnectionAsync();
                    return proyectos;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al descargar la base de datos de los proyectos:{ex.Message}");
                return new List<BasedeDatosDTO>();

            }
        }

        public async Task<List<ReporteBalanceDTO>> RecuperarBalanceHoras()
        {
            try
            {


                var balance = new List<ReporteBalanceDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "BalanceHoras";
                    command.CommandType = CommandType.StoredProcedure;
                   
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReporteBalanceDTO datos = new()
                            {
                                nombreproyecto = reader.IsDBNull(reader.GetOrdinal("NombreProyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreProyecto")),
                                numeroproyecto = reader.IsDBNull(reader.GetOrdinal("NumeroProyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("NumeroProyecto")),
                                tipologia = reader.IsDBNull(reader.GetOrdinal("Tipologia")) ? string.Empty : reader.GetString(reader.GetOrdinal("Tipologia")),
                                nombrecliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                tiporecurso = reader.IsDBNull(reader.GetOrdinal("TipoRecurso")) ? string.Empty : reader.GetString(reader.GetOrdinal("TipoRecurso")),
                                totalhorasporrecurso = reader.IsDBNull(reader.GetOrdinal("TotalHorasPorRecurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalHorasPorRecurso")),
                            };
                            balance.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return balance;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el balance de horas de los proyectos:{ex.Message}" );
                return new List<ReporteBalanceDTO>();

            }
        }
    }
}
