using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;

namespace Proyectogestionhoras.Services
{
    public class Masivo
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        public Masivo(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }
        public async Task GestorFechaModificacionProyectoMasivo(List<MoficacionProyectoViewModel> lista)
        {
            try
            {
                var idsConFechaPago = lista
                    .Where(m => m.FechaPago != null)
                    .Select(m => m.IdProyecto)
                    .ToList();


                var existentes = await context.Fechamodificacionproyectos
                    .Where(f => idsConFechaPago.Contains(f.Idproyecto))
                    .ToListAsync();

                TimeZoneInfo zonaChile = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
                DateTime fechaActual = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaChile);

                foreach (var idProyecto in idsConFechaPago)
                {
                    var existente = existentes.FirstOrDefault(f => f.Idproyecto == idProyecto);

                    if (existente != null)
                    {
                        existente.Fechamodificacion = fechaActual;
                        context.Fechamodificacionproyectos.Update(existente);
                    }
                    else
                    {
                        var nuevo = new Fechamodificacionproyecto
                        {
                            Idproyecto = idProyecto,
                            Fechamodificacion = fechaActual
                        };
                        await context.Fechamodificacionproyectos.AddAsync(nuevo);
                    }
                }


                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al modificar la fecha de proyecto modificación: {e.Message}");
            }
        }
    }
}
