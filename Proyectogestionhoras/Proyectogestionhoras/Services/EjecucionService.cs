using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;

namespace Proyectogestionhoras.Services
{
    public class EjecucionService : IEjecucion
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        public EjecucionService(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }

        public async Task GestorIngresos(int idproyecto, List<IngresoViewModel> ingresos)
        {
            if (ingresos == null || !ingresos.Any())
            {
                return;
            }

            foreach (var ingreso in ingresos)
            {
                
                var ingresoExistente = await context.Ingresosreales
                    .FirstOrDefaultAsync(i=>i.Id ==ingreso.Id);

                if (ingresoExistente != null)
                {

                    ingresoExistente.Numdocumento = ingreso.Numdocumento; 
                    ingresoExistente.FechaEmision = ingreso.FechaEmision;
                    ingresoExistente.FechaVencimiento = ingreso.FechaVencimiento;
                    ingresoExistente.Montous = ingreso.Montous;
                    ingresoExistente.Tc = ingreso.Tc;
                    ingresoExistente.Montoclp = ingreso.Montoclp;
                    ingresoExistente.Iva = ingreso.Iva;
                    ingresoExistente.Estado = ingreso.Estado;
                    ingresoExistente.Idcuenta = ingreso.Idcuenta;
                    ingresoExistente.FechaPago = ingreso.FechaPago;
                }
                else
                {
                    
                    var nuevoIngreso = new Ingresosreale
                    {
                        Idproyecto = idproyecto, 
                        Cuota = ingreso.Cuota + 1,
                        Numdocumento = ingreso.Numdocumento,
                        FechaEmision = ingreso.FechaEmision,
                        FechaVencimiento = ingreso.FechaVencimiento,
                        Montous = ingreso.Montous,
                        Tc = ingreso.Tc,
                        Montoclp = ingreso.Montoclp,
                        Iva = ingreso.Iva,
                        Estado = ingreso.Estado,
                        Idcuenta = ingreso.Idcuenta,
                        FechaPago = ingreso.FechaPago
                    };
                    await context.AddAsync(nuevoIngreso);
                }
            }

            // Guardamos todos los cambios realizados
            await context.SaveChangesAsync();
        }
    }
}
