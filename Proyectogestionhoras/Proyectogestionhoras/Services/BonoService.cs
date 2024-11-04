using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;

namespace Proyectogestionhoras.Services
{
    public class BonoService : IBono
    {
        private readonly Conexion conexion;
        public BonoService(Conexion conexion)
        {
            this.conexion = conexion;
        }
        public async Task<List<BonoDTO>> RecuperarDatosBonos(int anio)
        {
            try
            {


                var bonos = new List<BonoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERBONOTRIMESTREACTUAL";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@Anio", anio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            BonoDTO datos = new()
                            {
                                Monto = reader.IsDBNull(reader.GetOrdinal("MONTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("ANIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ANIO")),
                                Porcentaje = reader.IsDBNull(reader.GetOrdinal("PORCENTAJE")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PORCENTAJE")),
                                ValorReal = reader.IsDBNull(reader.GetOrdinal("VALORREAL")) ? 0 : reader.GetDecimal(reader.GetOrdinal("VALORREAL")),
                                ValorFinal = reader.IsDBNull(reader.GetOrdinal("VALORFINAL")) ? 0 : reader.GetDecimal(reader.GetOrdinal("VALORFINAL")),
                                Trimestre = reader.IsDBNull(reader.GetOrdinal("TRIMESTRE")) ? 0 : reader.GetInt32(reader.GetOrdinal("TRIMESTRE")),



                            };
                            bonos.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return bonos;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los bonos :" + ex.Message);
                return new List<BonoDTO>();

            }
        }
    }
}
