using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System.Data;
using System.Diagnostics;

namespace Proyectogestionhoras.Services
{
    public class PresupuestoService : IPresupuesto
    {
        private readonly Conexion conexion;
        public PresupuestoService(Conexion conexion)
        {
            this.conexion = conexion;
        }

        public async Task<bool> EditarPresupuesto(int idproyecto, decimal monto, decimal costosocio, decimal costostaff, decimal costoconsultora, decimal costoconsultorb, decimal consultorc, int? hhsocio, int? hhstaff, int? hhconsultora, int? hhconsultorb, int? hhconsultorc)
        {
            try
            {

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "EDITARPRESUPUESTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    command.Parameters.Add(new SqlParameter("@MONTO", monto));
                    command.Parameters.Add(new SqlParameter("@COSTOSOCIOS", costosocio));
                    command.Parameters.Add(new SqlParameter("@COSTOSTAFF", costostaff));
                    command.Parameters.Add(new SqlParameter("@COSTOCONSULTORA", costoconsultora));
                    command.Parameters.Add(new SqlParameter("@COSTOCONSULTORB", costoconsultorb));

                    command.Parameters.Add(new SqlParameter("@COSTOCONSULTORC", consultorc));
                    command.Parameters.Add(new SqlParameter("@HHSOCIOS", hhsocio));
                    command.Parameters.Add(new SqlParameter("@HHSTAFF", hhstaff));
                    command.Parameters.Add(new SqlParameter("@HHCONSULTORA", hhconsultora));
                    command.Parameters.Add(new SqlParameter("@HHCONSULTORB", hhconsultorb));

                    command.Parameters.Add(new SqlParameter("@HHCONSULTORC", hhconsultorc));
                    


                    await command.ExecuteNonQueryAsync();
                    

                    return true;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al editar el presupuesto" + ex);
                return false;
            }
        }
    }
}
