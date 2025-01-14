using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;


namespace Proyectogestionhoras.Services
{
    public class FacturaService : IFactura
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public FacturaService(Conexion conexion, PROYECTO_CONTROL_HORASContext context)
        {
            this.conexion = conexion;
            this.context = context;
        }

        public async Task<List<FacturaProyectoDTO>> RecuperarFacturas(int? idproyecto)
        {
            try
            {
                var facturas = new List<FacturaProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CUOTASPROYECTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FacturaProyectoDTO factura = new()
                            {
                                IdCuenta = reader.GetInt32(reader.GetOrdinal("IdCuenta")),
                                Cuenta = reader.GetString(reader.GetOrdinal("Cuenta")),
                                Segmento = reader.GetString(reader.GetOrdinal("Segmento")),
                                Fecha_Factura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                Neto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                
                            };
                            facturas.Add(factura);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return facturas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los sesrvicios del proyecto:" + ex.Message);
                return new List<FacturaProyectoDTO>();
            }
        }


        public async Task<List<FacturaProyectoDTO>> RecuperarFacturasPresupuesto(int? idproyecto)
        {
            try
            {
                var facturas = new List<FacturaProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GENERARFACTURAPROYECTOPRESUPUUESTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPRESUPUESTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FacturaProyectoDTO factura = new()
                            {
                                IdCuenta = reader.GetInt32(reader.GetOrdinal("IdCuenta")),
                                Cuenta = reader.GetString(reader.GetOrdinal("Cuenta")),
                                Segmento = reader.GetString(reader.GetOrdinal("Segmento")),
                                Fecha_Factura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                Neto = reader.GetDecimal(reader.GetOrdinal("Neto")),
                                IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            };
                            facturas.Add(factura);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return facturas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las facturas del presupuesto del proyecto:" + ex.Message);
                return new List<FacturaProyectoDTO>();
            }
        }
    }
}
