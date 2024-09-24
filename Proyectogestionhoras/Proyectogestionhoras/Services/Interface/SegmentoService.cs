using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Services.Interface
{
    public class SegmentoService : ISegmento
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public SegmentoService(Conexion conexion, PROYECTO_CONTROL_HORASContext context)
        {
            this.conexion = conexion;
            this.context = context;
        }
       /* public async Task<List<SegmentoCostoDTO>> ObtenerSegmentosCostos(int? id, int? idcuenta, int? idsegmento)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                object idcuentaparameter = (object)idcuenta ?? DBNull.Value;
                object idsegmentoparameter = (object)idsegmento ?? DBNull.Value;
                #pragma warning restore CS8600
                var segmentos = new List<SegmentoCostoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGCOSTCUENTA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ID", idparameter));
                    command.Parameters.Add(new SqlParameter("@IDCUENTA", idcuentaparameter));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTO", idsegmentoparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SegmentoCostoDTO segmento = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                NOMBRESEGMENTO = reader.GetString(reader.GetOrdinal("NOMBRESEGMENTO")),
                                CENTRO_COSTOS = reader.GetString(reader.GetOrdinal("CENTRO_COSTOS")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),

                            };
                            segmentos.Add(segmento);
                            
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return segmentos;

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Error de operación inválida: {ex.Message}");
                return new List<SegmentoCostoDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los segmentos costos:" + ex.Message);
                return new List<SegmentoCostoDTO>();
            }
        }*/
    }
}
