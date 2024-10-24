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
        public async Task<List<MetaFacturacionqxDTO>> GetMetaFacturacionqx()
        {
            try
            {

                var facturas = new List<MetaFacturacionqxDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERMETAFACTURACIONQX";
                    command.CommandType = CommandType.StoredProcedure;
               
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MetaFacturacionqxDTO datos = new()
                            {
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
    }
}
