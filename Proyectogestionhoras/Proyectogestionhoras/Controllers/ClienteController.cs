using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using System.Diagnostics;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Proyectogestionhoras.Controllers
{
    public class ClienteController : Controller
    {
        private readonly PROYECTO_CONTROL_HORASContext _context;
        private readonly Conexion conexion; 

        public ClienteController(PROYECTO_CONTROL_HORASContext context, Conexion conexion)
        {
            _context = context;
            this.conexion = conexion;
        }

        public IActionResult DetalleCliente(int? id,string? nombre,string? pais)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCliente()
        {
            try
            {
                string nombre = Request.Form["nombre"].ToString() ?? string.Empty;
                string direccion = Request.Form["direccion"].ToString() ?? string.Empty;
                string ciudad = Request.Form["ciudad"].ToString() ?? string.Empty;
                string pais = Request.Form["pais"].ToString() ?? string.Empty;
                string telefono = Request.Form["telefono"].ToString() ?? string.Empty;
                string pagweb = Request.Form["pagweb"].ToString() ?? string.Empty;
                string linkedin = Request.Form["linkedin"].ToString() ?? string.Empty;
                string instagram = Request.Form["instagram"].ToString() ?? string.Empty;
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REGISTRARCLIENTE";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@DIRECCION", direccion));
                    command.Parameters.Add(new SqlParameter("@CIUDAD",ciudad));
                    command.Parameters.Add(new SqlParameter("@PAIS", pais));
                    command.Parameters.Add(new SqlParameter("@TELEFONO", telefono));
                    command.Parameters.Add(new SqlParameter("@PAG_WEB", pagweb));
                    command.Parameters.Add(new SqlParameter("@LINKEDIN", linkedin));
                    command.Parameters.Add(new SqlParameter("@INSTAGRAM", instagram));
                    await command.ExecuteNonQueryAsync();
                    await conexion.CloseDatabaseConnectionAsync();
                }
                TempData["SuccessMessage"] = "¡Se Agregó con éxito el nuevo cliente!";
                return RedirectToAction("Index", "Home");
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar el cliente"+ex.Message);
                return View();
            }
        }
    }

}
