using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using OfficeOpenXml.Drawing.Chart;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Proyectogestionhoras.Models.DTO;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Proyectogestionhoras.Services
{
    public class MetaService : IMeta
    {
        private readonly Conexion conexion;
        public MetaService(Conexion conexion)
        {
            this.conexion = conexion;
        }
        public async Task<bool> RegistrarMetaFacturacion(int anio, decimal q1, decimal q2, decimal q3, decimal q4)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INGRESARFACTURAQX";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));
                    command.Parameters.Add(new SqlParameter("@MONTOQ1", q1));
                    command.Parameters.Add(new SqlParameter("@MONTOQ2", q2));
                    command.Parameters.Add(new SqlParameter("@MONTOQ3", q3));
                    command.Parameters.Add(new SqlParameter("@MONTOQ4", q4));
                    
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al registrar la meta de facturación" + ex);
                return false;
            }
        }

        public async Task<bool> ActualizarMetaFactura(int id, int anio, decimal q1, decimal q2, decimal q3, decimal q4)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ACTUALIZARMETAFACTURA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));
                    command.Parameters.Add(new SqlParameter("@MONTOQ1", q1));
                    command.Parameters.Add(new SqlParameter("@MONTOQ2", q2));
                    command.Parameters.Add(new SqlParameter("@MONTOQ3", q3));
                    command.Parameters.Add(new SqlParameter("@MONTOQ4", q4));

                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al actualizar la meta de facturación" + ex);
                return false;
            }
        }

        public async Task<List<MetaFacturacionqxDTO>> GetMetaFacturacionqx(int id)
        {
            try
            {

                var facturas = new List<MetaFacturacionqxDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERMETAFACTURACIONQX";
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    command.CommandType = CommandType.StoredProcedure;
               
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MetaFacturacionqxDTO datos = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Anio = reader.GetInt32(reader.GetOrdinal("ANIO")),
                                q1 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q1")),
                                q2 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q2")),
                                q3 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q3")),
                                q4 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q4")),
                                totalq = reader.GetDecimal(reader.GetOrdinal("TotalMontoQ")),
                               
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

                Debug.WriteLine($"Hubo un error al obtener las metas de la facturas:" + ex.Message);
                return new List<MetaFacturacionqxDTO>();

            }
        }
        public async Task<bool> RegistrarMetaTipologia(int anio, int t1, int t2, int t3, int t4, int totalproyecto, decimal totalporproyecto, int duracion, decimal montomensual)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INGRESARMETATIPOLOGIA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));
                    command.Parameters.Add(new SqlParameter("@T1", t1));
                    command.Parameters.Add(new SqlParameter("@T2", t2));
                    command.Parameters.Add(new SqlParameter("@T3", t3));
                    command.Parameters.Add(new SqlParameter("@T4", t4));
                    command.Parameters.Add(new SqlParameter("@TOTALPROYECTO", totalproyecto));
                    command.Parameters.Add(new SqlParameter("@TOTALPORPROYECTO", totalporproyecto));
                    command.Parameters.Add(new SqlParameter("@DURACIONMEDIA", duracion));
                    command.Parameters.Add(new SqlParameter("@MONTOMENSUAL", montomensual));

                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al registrar la meta de facturación" + ex);
                return false;
            }
        }
        public async Task<List<MetasTipologiaDTO>> GetMetasTipologias(int id)
        {
            try
            {

                var tipologias = new List<MetasTipologiaDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERMETASTIPOLOGIAS";
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MetasTipologiaDTO datos = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdMeta")),
                                Anio = reader.GetInt32(reader.GetOrdinal("ANIO")),
                                T1 = reader.GetInt32(reader.GetOrdinal("T1")),
                                T2 = reader.GetInt32(reader.GetOrdinal("T2")),
                                T3 = reader.GetInt32(reader.GetOrdinal("T3")),
                                T4 = reader.GetInt32(reader.GetOrdinal("T4")),
                                TotalProyecto = reader.GetInt32(reader.GetOrdinal("TOTALPROYECTO")),
                                TotalPorProyecto = reader.GetDecimal(reader.GetOrdinal("TOTALPORPROYECTO")),
                                DuracionMedia = reader.GetInt32(reader.GetOrdinal("DURACIONMEDIA")),
                                MontoMensual = reader.GetDecimal(reader.GetOrdinal("TOTALMENSUALES")),
                                totalfactura = reader.GetDecimal(reader.GetOrdinal("TOTALFACTURA")),

                            };
                            tipologias.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return tipologias;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las metas de la tipologias:" + ex.Message);
                return new List<MetasTipologiaDTO>();

            }
        }
        public async Task<bool> ActualizarMetaTipologia(int id, int anio, int t1, int t2, int t3, int t4)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "ACTUALIZARMETATIPOLOGIA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", id));
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));
                    command.Parameters.Add(new SqlParameter("@T1", t1));
                    command.Parameters.Add(new SqlParameter("@T2", t2));
                    command.Parameters.Add(new SqlParameter("@T3", t3));
                    command.Parameters.Add(new SqlParameter("@T4", t4));
                    

                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al registrar la meta de facturación" + ex);
                return false;
            }
        }
        public async Task<List<DatosMetasDTO>> GetDatosMeta(int anio)
        {
            try
            {

                var facturas = new List<DatosMetasDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "TRAERDATOSMETASANIO";
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DatosMetasDTO datos = new()
                            {
                                Factura = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                Totalproyecto = reader.GetDecimal(reader.GetOrdinal("TOTALPORPROYECTO")),
                                Duracion = reader.GetInt32(reader.GetOrdinal("DURACIONMEDIA")),
                                

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

                Debug.WriteLine($"Hubo un error al obtener las metas de la facturas:" + ex.Message);
                return new List<DatosMetasDTO>();

            }
        }
        public async Task<List<MetaQuarterFacturaDTO>> GetDatosMetafacturaq(int anio)
        {
            try
            {

                var facturas = new List<MetaQuarterFacturaDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM META_FACTURACIONESQX WHERE anio = @ANIO";
                    command.CommandType = CommandType.Text; 
                    command.Parameters.Add(new SqlParameter("@ANIO", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MetaQuarterFacturaDTO datos = new()
                            {
                                montoq1 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q1")),
                                montoq2 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q2")),
                                montoq3 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q3")),
                                montoq4 = reader.GetDecimal(reader.GetOrdinal("MONTO_Q4")),
                                


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

                Debug.WriteLine($"Hubo un error al obtener las metas de la facturas:" + ex.Message);
                return new List<MetaQuarterFacturaDTO>();

            }
        }

    }
}
