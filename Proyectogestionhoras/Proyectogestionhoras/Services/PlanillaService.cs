using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;
namespace Proyectogestionhoras.Services
{
    public class PlanillaService : IPlanilla
    {
        private readonly PROYECTO_CONTROL_HORASContext context;

        public PlanillaService(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }


        public async Task<int> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones, int idactividad)
        {
            try
            {
                // Buscar o crear la planilla
                var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == idusuario);
                if (planilla == null)
                {
                    planilla = new Planilla
                    {
                        IdUsuario = idusuario,
                    };
                    context.Planillas.Add(planilla);
                    await context.SaveChangesAsync();
                }

                // Verificar si ya existe un registro para ese día
                bool existereigstro = await context.PlanillaUsusarioProyectos.AnyAsync(p => p.IdUsuProy == idusuproy && p.FechaRegistro.Date == Fecharegistro.Date);
                if (existereigstro)
                {
                    return 2; // Registro ya existente
                }

                // Crear el nuevo registro de horas
                var registro = new PlanillaUsusarioProyecto
                {
                    IdPlanilla = planilla.Id,
                    IdUsuProy = idusuproy,
                    RegistroHhProyecto = horasasignadas,
                    FechaRegistro = Fecharegistro,
                    Observaciones = observaciones,
                    IdActividad = idactividad,
                };
                context.PlanillaUsusarioProyectos.Add(registro);

                // Buscar el UsuarioProyecto
                var usuarioproyecto = await context.UsuarioProyectos
                .Include(up => up.IdUsuarioNavigation) // Asegúrate de incluir la navegación al usuario
                .ThenInclude(u => u.IdRecursoNavigation) // También incluye el recurso
                .FirstOrDefaultAsync(up => up.Id == idusuproy);

                if (usuarioproyecto != null)
                {
                    
                    if (usuarioproyecto.HhSocios.HasValue)
                    {
                        usuarioproyecto.HhSocios -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhStaff.HasValue)
                    {
                        usuarioproyecto.HhStaff -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultora.HasValue)
                    {
                        usuarioproyecto.HhConsultora -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultorb.HasValue)
                    {
                        usuarioproyecto.HhConsultorb -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultorc.HasValue)
                    {
                        usuarioproyecto.HhConsultorc -= horasasignadas;
                    }

                   
                    var usuario = usuarioproyecto.IdUsuarioNavigation;
                    
                   
                    if (usuario != null && usuario.IdRecurso != 0)
                    {
                        var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                        if (recurso != null)
                        {
                        
                            if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                            {
                                recurso.HhAnuales += horasasignadas; 
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                            return 2;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("El usuario no es válido o no tiene un recurso asociado.");
                        return 2;
                    }

                    await context.SaveChangesAsync();
                    return 1; // Registro exitoso
                }
                else
                {
                    Debug.WriteLine("Error: UsuarioProyecto no se encontró.");
                    return 2; // Manejo de error si UsuarioProyecto no se encuentra
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar la hora en la planilla: {ex}");
                return 2; // Manejo de error general
            }
        }



    }
}
