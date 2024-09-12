using Proyectogestionhoras.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Proyectogestionhoras.Models
{
    public class Conexion
    {
        private readonly PROYECTO_CONTROL_HORASContext context;

        public Conexion(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }

        public async Task<DbConnection> OpenDatabaseConnectionAsync()
        {
            try
            {

                await context.Database.OpenConnectionAsync();

                Debug.WriteLine("¡Éxito en la conexión!");
                return context.Database.GetDbConnection();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al abrir la conexión a la base de datos: {ex.Message}");
                throw;
            }
        }
        public async Task CloseDatabaseConnectionAsync()
        {
            try
            {

                await context.Database.CloseConnectionAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al cerrar la conexión a la base de datos: {ex.Message}");
                throw;
            }
        }
    }
}