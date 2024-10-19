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
    }
}
