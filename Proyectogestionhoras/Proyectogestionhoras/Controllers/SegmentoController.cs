using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Proyectogestionhoras.Controllers
{
    public class SegmentoController : Controller
    {
        private readonly SegmentoService segmento;

        private readonly Conexion conexion;
        
        public SegmentoController(SegmentoService segmento,Conexion conexion)
        {
            this.segmento = segmento;
            this.conexion = conexion;
        }

        public async Task<IActionResult> GetSegmentoCostos()
        {
            var cuentas = await getcuentas();
            var segmentos = await getSEGMENTO();
            ViewBag.Ccostos = cuentas;
            ViewBag.Segmentos = segmentos;
            return View("Segmentos");
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int segmento,int ccosto)
        {
            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "REGISTARSEGMENTOCCOSTO";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@IDSEGMENTO", segmento));
                command.Parameters.Add(new SqlParameter("@IDCCOSTO", ccosto));
                
               
                await command.ExecuteNonQueryAsync();
                await conexion.CloseDatabaseConnectionAsync();
                return RedirectToAction("GetSegmentoCostos","Segmento");
            }
    
        }


        public async Task<List<CcostoUnegocio>> getcuentas() {

            var ccostos = new List<CcostoUnegocio>();
            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "OBTENERCCOSTOUNEGOCIO";
                command.CommandType = CommandType.StoredProcedure;
              
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CcostoUnegocio ccosto = new()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),
                          

                        };
                        ccostos.Add(ccosto);

                    }
                }

            }
            await conexion.CloseDatabaseConnectionAsync();
            return ccostos;


        }
        public async Task<List<Segmento>> getSEGMENTO()
        {

            var segmentos = new List<Segmento>();
            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "OBTENERSEGMENTO";
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Segmento segmento = new()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),


                        };
                        segmentos.Add(segmento);

                    }
                }

            }
            await conexion.CloseDatabaseConnectionAsync();
            return segmentos;


        }
    }
}
