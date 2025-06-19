using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using iText.Commons.Actions.Contexts;
using iText.Layout.Element;
using iText.Bouncycastle.Asn1;
namespace Proyectogestionhoras.Services
{
    public class PlanillaService : IPlanilla
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly Conexion conexion;
        public PlanillaService(PROYECTO_CONTROL_HORASContext context, Conexion conexion)
        {
            this.context = context;
            this.conexion = conexion;
        }

        public async Task<int> RegistrarHorasPlanills(int idusuario, int? idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones, int Idactividad, int? idsubactividad)
        {
            try
            {
                int resultado = 0;
                if (idusuproy.HasValue && idusuproy > 0 && (!idsubactividad.HasValue || idsubactividad == 0))
                {
                    resultado = await RegistrarHoras(idusuario, idusuproy.Value, horasasignadas, Fecharegistro, observaciones, Idactividad);
                }
                else if (idsubactividad.HasValue && idsubactividad > 0 && (!idusuproy.HasValue || idusuproy == 0))
                {
                    resultado = await RegistrarHorasEmpresa(idusuario, horasasignadas, Fecharegistro, observaciones, idsubactividad.Value);
                }
                return resultado;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al registrar horas en la planilla:{e.Message}");
                return 0;
            }
        }



        public async Task<int> RegistrarHoras(int idusuario, int idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones, int Idactividad)
        {
            try
            {
                int statusProyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == idusuproy).Select(up => up.IdProyectoNavigation.StatusProyecto).FirstOrDefaultAsync();
                string tipo = string.Empty;

                if (statusProyecto == 2)
                {
                    tipo = "En Ejecución";
                }
                else if (statusProyecto == 1)
                {
                    tipo = "En Negociación";
                }
                else
                {
                    return 2;
                }
                bool existereigstro = await context.PlanillaUsusarioProyectos.AnyAsync(p => p.IdUsuProy == idusuproy && p.FechaRegistro.Date == Fecharegistro.Date && p.Idactividad == Idactividad);
                if (existereigstro)
                {
                    return 2;
                }
                /* var proyecto = ObtenerProyectoPorUsuarioProyectoId(idusuproy);
                 DateTime fechainicio = proyecto.FechaInicio;
                 DateTime fechatermino = proyecto.FechaTermino;
                 if (Fecharegistro < fechainicio || Fecharegistro > fechatermino)
                 {
                     return 4;
                 }*/
                decimal horasAsignadasDecimal;
                if (!decimal.TryParse(horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                {

                    return 0;
                }
                int mesregistro = Fecharegistro.Month;
                int anioregistro = Fecharegistro.Year;

                var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                if (planilla == null)
                {
                    planilla = new Planilla
                    {
                        IdUsuario = idusuario,
                        Mes = mesregistro,
                        Anio = anioregistro,
                    };
                    context.Planillas.Add(planilla);
                    await context.SaveChangesAsync();
                }

                var registro = new PlanillaUsusarioProyecto
                {
                    IdPlanilla = planilla.Id,
                    IdUsuProy = idusuproy,
                    RegistroHhProyecto = horasAsignadasDecimal,
                    FechaRegistro = Fecharegistro,
                    Observaciones = observaciones,
                    Idactividad = Idactividad,
                    Tipo = tipo,
                };
                context.PlanillaUsusarioProyectos.Add(registro);

                var inicioSemana = Fecharegistro.AddDays(-(int)Fecharegistro.DayOfWeek + (int)DayOfWeek.Monday);
                var finSemana = inicioSemana.AddDays(6);
                var horasRegistradasSemana = await context.PlanillaUsusarioProyectos
                    .Join(
                    context.UsuarioProyectos,
                    planilla => planilla.IdUsuProy,
                    usuarioProyecto => usuarioProyecto.Id,
                    (planilla, usuarioProyecto) => new { planilla, usuarioProyecto }
                    )
                    .Where(joinResult => joinResult.usuarioProyecto.IdUsuario == idusuario
                         && joinResult.planilla.FechaRegistro >= inicioSemana
                         && joinResult.planilla.FechaRegistro <= finSemana)
                    .SumAsync(joinResult => joinResult.planilla.RegistroHhProyecto);


                var usuarioproyecto = await context.UsuarioProyectos
                .Include(up => up.IdUsuarioNavigation)
                .ThenInclude(u => u.IdRecursoNavigation)
                .FirstOrDefaultAsync(up => up.Id == idusuproy);
                if (statusProyecto == 1)
                {
                    await context.SaveChangesAsync();
                    return 1;
                }
                if (statusProyecto == 2)
                {
                    int idproyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == idusuproy).Select(up => up.IdProyectoNavigation.Id).FirstOrDefaultAsync();
                    var usuario = usuarioproyecto.IdUsuarioNavigation;
                    var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);

                    await context.SaveChangesAsync();
                    await gestorhhplanilla(idproyecto, recurso.NombreRecurso, horasAsignadasDecimal, mesregistro, anioregistro, 0);
                }

                /*
                if (usuarioproyecto != null)
                {


                     if (usuarioproyecto.HhConsultora.HasValue)
                     {
                         usuarioproyecto.HhConsultora -= horasAsignadasDecimal;
                     }
                     else if (usuarioproyecto.HhConsultorb.HasValue)
                     {
                         usuarioproyecto.HhConsultorb -= horasAsignadasDecimal;
                     }
                     else if (usuarioproyecto.HhConsultorc.HasValue)
                     {
                         usuarioproyecto.HhConsultorc -= horasAsignadasDecimal;
                     }


                     var usuario = usuarioproyecto.IdUsuarioNavigation;

                     if (statusProyecto == 2)
                     {
                         if (usuario != null && usuario.IdRecurso != 0)
                         {
                             var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                             if (recurso != null)
                             {

                                 if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                                 {

                                     if (recurso.NombreRecurso == "Socio")
                                     {


                                         var usuariosRelacionados = await context.UsuarioProyectos
                                         .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                         .ToListAsync();


                                         foreach (var usuarioRelacionado in usuariosRelacionados)
                                         {
                                             usuarioRelacionado.HhSocios -= horasAsignadasDecimal;
                                         }
                                     }

                                     else if (recurso.NombreRecurso == "Staff")
                                     {


                                         var usuariosRelacionados = await context.UsuarioProyectos
                                         .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                         .ToListAsync();


                                         foreach (var usuarioRelacionado in usuariosRelacionados)
                                         {
                                             usuarioRelacionado.HhStaff -= horasAsignadasDecimal;
                                         }
                                     }


                                      decimal? totalpermitidossemana = recurso.NumeroHoras * (recurso.ProcentajeProyecto / 100);
                                      Debug.WriteLine(totalpermitidossemana);
                                      if (horasRegistradasSemana + horasAsignadasDecimal > totalpermitidossemana)
                                      {
                                          Debug.WriteLine("Error: Se exceden las horas permitidas en la semana.");
                                          return 3;
                                      }




                                 }
                             }
                         }

                         else
                         {
                             Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                             return 2;
                         }
                     }


                 }
                 else
                 {
                     Debug.WriteLine("Error: UsuarioProyecto no se encontró.");
                     return 2;
                 }
                    */

                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar la hora en la planilla: {ex}");
                return 2;
            }
        }


        public async Task gestorhhplanilla(int idproyecto, string tiporecurso, decimal hh, int mes, int anio, int eliminar, int? proyectoorigen = null,DateTime? fechaorigen = null)
        {

            var idregistro = await context.Gastoshhhejecucions.Where(g => g.Mes == mes && g.Anio == anio && g.Idproyecto == idproyecto && g.Tiporecurso == tiporecurso).Select(g => g.Id).FirstOrDefaultAsync();
            decimal? costoproyecto = 0;

            if (idproyecto != 0)
            {
                if (tiporecurso == "Socio")
                {
                    costoproyecto = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costosocio).FirstOrDefaultAsync();
                }
                else if (tiporecurso == "Staff")
                {
                    costoproyecto = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costostaff).FirstOrDefaultAsync();
                }
            }

            bool esTraspaso = proyectoorigen.HasValue && proyectoorigen.Value != idproyecto;
            Debug.WriteLine("souy"+esTraspaso);

            if (idregistro > 0)
            {
                var cantidadhoraplanillas = await (from p in context.PlanillaUsusarioProyectos
                                                   join up in context.UsuarioProyectos on p.IdUsuProy equals up.Id
                                                   join pro in context.Proyectos on up.IdProyecto equals pro.Id
                                                   join usu in context.Usuarios on up.IdUsuario equals usu.Id
                                                   join r in context.Recursos on usu.IdRecurso equals r.Id
                                                   join pla in context.Planillas on p.IdPlanilla equals pla.Id
                                                   where (pro.Id == idproyecto && r.NombreRecurso == tiporecurso && pla.Mes == mes && pla.Anio == anio)
                                                   select p.RegistroHhProyecto).SumAsync();



                var gastoshh = await context.Gastoshhhejecucions.FindAsync(idregistro);
                if (gastoshh.Estado == 0 || gastoshh.Estado == 1)
                {
                    bool eliminadoPrincipal = false;
                    bool eliminadoOrigen = false;

                    if (eliminar == 1)
                    {
                        gastoshh.Hhtotales -= hh;

                        if (gastoshh.Hhtotales <= 0)
                        {
                            context.Gastoshhhejecucions.Remove(gastoshh);
                            eliminadoPrincipal = true;
                        }

                        
                    }
                    else if (eliminar == 2)
                    {
                        /*distinto proyecto , pero mismo mes*/
                        if (esTraspaso)
                        {
                            Debug.WriteLine($"estoy entranco aca???");
                            gastoshh.Hhtotales += hh;

                            var registroorigen = await context.Gastoshhhejecucions
                                .Where(g => g.Mes == mes && g.Anio == anio && g.Idproyecto == proyectoorigen && g.Tiporecurso == tiporecurso).FirstOrDefaultAsync();

                            if (registroorigen != null)
                            {
                                registroorigen.Hhtotales -= hh;
                                if (registroorigen.Hhtotales <= 0)
                                {
                                    context.Gastoshhhejecucions.Remove(registroorigen);
                                    eliminadoOrigen = true;
                                }
                                else
                                {
                                    registroorigen.Monto = costoproyecto * registroorigen.Hhtotales;
                                    registroorigen.Subtotal = costoproyecto * registroorigen.Hhtotales;
                                    context.Update(registroorigen);
                                }
                            }
                        }
                        else
                        {
                            gastoshh.Hhtotales = cantidadhoraplanillas;
                        }
                    }
                    else
                    {
                        gastoshh.Hhtotales += hh;
                    }

                    if (!eliminadoPrincipal)
                    {
                        gastoshh.Monto = costoproyecto * gastoshh.Hhtotales;
                        gastoshh.Subtotal = costoproyecto * gastoshh.Hhtotales;
                        context.Update(gastoshh);
                    }
                }

            }
            else
            {
                var nuevogasto = new Gastoshhhejecucion
                {
                    Mes = mes,
                    Anio = anio,
                    Idproyecto = idproyecto,
                    Tiporecurso = tiporecurso,
                    Hhtotales = hh,
                    Estado = 0,
                    Subtotal = costoproyecto * hh,
                    Monto = costoproyecto * hh,
                    Reajuste = 0,
                };
                await context.AddAsync(nuevogasto);





            }


            /*quitando horas a fecha antigua si hay traspaso de mes a otro y si idregistro es 0*/
            if (eliminar == 2)
            {
                if(fechaorigen != null)
                {
                    int mesantiguo = fechaorigen.Value.Month;
                    int anioantiguo = fechaorigen.Value.Year;

                    if (esTraspaso && idregistro == 0)
                    {
                        var idregistrohistorico = await context.Gastoshhhejecucions.Where(g => g.Mes == mesantiguo && g.Anio == anioantiguo && g.Idproyecto == proyectoorigen && g.Tiporecurso == tiporecurso).Select(g => g.Id).FirstOrDefaultAsync();
                        if (idregistrohistorico > 0)
                        {
                            Debug.WriteLine("estoy entrando ca en origen");

                            Debug.WriteLine("ahora voy ca en origen");
                            var gastoshhantiguo = await context.Gastoshhhejecucions.FindAsync(idregistrohistorico);
                            gastoshhantiguo.Hhtotales -= hh;

                            if (gastoshhantiguo.Hhtotales <= 0)
                            {
                                context.Gastoshhhejecucions.Remove(gastoshhantiguo);
                                Debug.WriteLine("eliminandome origen");
                            }
                            else
                            {
                                gastoshhantiguo.Monto = costoproyecto * gastoshhantiguo.Hhtotales;
                                gastoshhantiguo.Subtotal = costoproyecto * gastoshhantiguo.Hhtotales;
                                context.Update(gastoshhantiguo);
                                Debug.WriteLine("actualizandome origen");
                            }



                        }
                    }
                    else
                    {
                        /*mismo proyecto, pero distinto mes*/
                        var idregistrohistorico = await context.Gastoshhhejecucions.Where(g => g.Mes == mesantiguo && g.Anio == anioantiguo && g.Idproyecto == idproyecto && g.Tiporecurso == tiporecurso).Select(g => g.Id).FirstOrDefaultAsync();
                        if (idregistrohistorico > 0)
                        {
                            Debug.WriteLine("estoy entrando ca");
                            if (mes != mesantiguo)
                            {

                                Debug.WriteLine("ahora voy ca");
                                var gastoshhantiguo = await context.Gastoshhhejecucions.FindAsync(idregistrohistorico);
                                gastoshhantiguo.Hhtotales -= hh;

                                if (gastoshhantiguo.Hhtotales <= 0)
                                {
                                    context.Gastoshhhejecucions.Remove(gastoshhantiguo);
                                    Debug.WriteLine("eliminandome");
                                }
                                else
                                {
                                    gastoshhantiguo.Monto = costoproyecto * gastoshhantiguo.Hhtotales;
                                    gastoshhantiguo.Subtotal = costoproyecto * gastoshhantiguo.Hhtotales;
                                    context.Update(gastoshhantiguo);
                                    Debug.WriteLine("actualizandome");
                                }


                            }

                        }
                    }
                }
               

            }


            await context.SaveChangesAsync();
        }


        public Proyecto ObtenerProyectoPorUsuarioProyectoId(int idUsuarioProyecto)
        {


            var usuarioProyecto = context.UsuarioProyectos
                .Include(up => up.IdProyectoNavigation)
                .FirstOrDefault(up => up.Id == idUsuarioProyecto);


            return usuarioProyecto?.IdProyectoNavigation;

        }


        public async Task<int> RegistrarHorasEmpresa(int idusuario, string horasasignadas, DateTime Fecharegistro, string? observaciones, int idsubactividad)
        {

            try
            {
                decimal? costoUnitario = 0;
                if (idsubactividad != 12 && idsubactividad != 13 & idsubactividad != 17 && idsubactividad != 18)
                {
                    costoUnitario = await context.Recursos.Where(r => r.Usuarios.Any(u => u.Id == idusuario)).Select(r => r.CostoUnitario).FirstOrDefaultAsync();
                }
                bool existereigstro = await context.PlanillaRegistroEmpresas.AnyAsync(p => p.Idsubactividad == idsubactividad && p.Fecharegistro == Fecharegistro.Date && p.IdPlanillaNavigation.IdUsuarioNavigation.Id == idusuario);
                if (existereigstro)
                {
                    return 2;
                }

                decimal horasAsignadasDecimal;
                if (!decimal.TryParse(horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                {

                    return 0;
                }
                int mesregistro = Fecharegistro.Month;
                int anioregistro = Fecharegistro.Year;

                var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                if (planilla == null)
                {
                    planilla = new Planilla
                    {
                        IdUsuario = idusuario,
                        Mes = mesregistro,
                        Anio = anioregistro,
                    };
                    context.Planillas.Add(planilla);
                    await context.SaveChangesAsync();
                }

                var registro = new PlanillaRegistroEmpresa
                {
                    IdPlanilla = planilla.Id,
                    Hhregistradas = horasAsignadasDecimal,
                    Fecharegistro = Fecharegistro,
                    Observaciones = observaciones,
                    Idsubactividad = idsubactividad,
                    CostoMonetario = costoUnitario,
                    Correlativo = GenerarCorrelativo()
                };
                context.PlanillaRegistroEmpresas.Add(registro);
                await context.SaveChangesAsync();
                return 1;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar la hora en la planilla: {ex}");
                return 2;
            }
        }
        public string GenerarCorrelativo(string letra = "A")
        {

            var ultimoRegistro = ObtenerUltimoRegistroCorrelativo();

            if (ultimoRegistro == null)
            {
                return letra + "1";
            }

            var ultimoNumero = int.Parse(ultimoRegistro.Correlativo.Substring(1));
            return letra + (ultimoNumero + 1).ToString();
        }
        public PlanillaRegistroEmpresa ObtenerUltimoRegistroCorrelativo()
        {

            return context.PlanillaRegistroEmpresas
                .OrderByDescending(r => r.Correlativo)
                .FirstOrDefault();
        }

        public async Task<int> EditarRegistro([FromBody] EditarRegistroViewModel editarregistro)
        {
            try
            {
                int resultado = 0;
                int subactividadhistorica = await context.PlanillaRegistroEmpresas.Where(p => p.Correlativo == editarregistro.correlativo).Select(p => p.Idsubactividad).FirstOrDefaultAsync();
                if (subactividadhistorica > 0)
                {
                    if (editarregistro.idsubactividad > 0 && (editarregistro.idusuproy == 0))
                    {
                        var registro = await context.PlanillaRegistroEmpresas.FindAsync(editarregistro.idregistro);
                        if (registro != null)
                        {

                            decimal? costoUnitario = 0;
                            if (editarregistro.idsubactividad != 12 && editarregistro.idsubactividad != 13 & editarregistro.idsubactividad != 17 && editarregistro.idsubactividad != 18)
                            {
                                costoUnitario = await context.Recursos.Where(r => r.Usuarios.Any(u => u.Id == editarregistro.idusuario)).Select(r => r.CostoUnitario).FirstOrDefaultAsync();
                            }




                            decimal horasAsignadasDecimal;
                            if (!decimal.TryParse(editarregistro.horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                            {

                                return 0;
                            }


                            int mesregistro = editarregistro.Fecharegistro.Month;
                            int anioregistro = editarregistro.Fecharegistro.Year;

                            var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == editarregistro.idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                            if (planilla == null)
                            {
                                planilla = new Planilla
                                {
                                    IdUsuario = editarregistro.idusuario,
                                    Mes = mesregistro,
                                    Anio = anioregistro,
                                };
                                context.Planillas.Add(planilla);
                                await context.SaveChangesAsync();
                            }


                            registro.IdPlanilla = planilla.Id;
                            registro.Fecharegistro = editarregistro.Fecharegistro;
                            registro.Idsubactividad = editarregistro.idsubactividad;
                            registro.Hhregistradas = horasAsignadasDecimal;
                            registro.Observaciones = editarregistro.observaciones;
                            registro.CostoMonetario = costoUnitario;
                            await context.SaveChangesAsync();

                        }
                        resultado = 1;
                    }


                    /*EDITANDO PROYECTO CUANDO ANTES ERA SUBACTIVIDAD*/


                    else if (editarregistro.idsubactividad == 0 && editarregistro.idusuproy > 0)
                    {
                        int statusProyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == editarregistro.idusuproy).Select(up => up.IdProyectoNavigation.StatusProyecto).FirstOrDefaultAsync();
                        string tipo = string.Empty;

                        if (statusProyecto == 2)
                        {
                            tipo = "En Ejecución";
                        }
                        else if (statusProyecto == 1)
                        {
                            tipo = "En Negociación";
                        }
                        else
                        {
                            return 2;
                        }
                        var registro = await context.PlanillaRegistroEmpresas.FindAsync(editarregistro.idregistro);
                        decimal horasAsignadasDecimal;
                        if (!decimal.TryParse(editarregistro.horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                        {

                            return 0;
                        }

                        int mesregistro = editarregistro.Fecharegistro.Month;
                        int anioregistro = editarregistro.Fecharegistro.Year;

                        var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == editarregistro.idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                        if (planilla == null)
                        {
                            planilla = new Planilla
                            {
                                IdUsuario = editarregistro.idusuario,
                                Mes = mesregistro,
                                Anio = anioregistro,
                            };
                            context.Planillas.Add(planilla);
                            await context.SaveChangesAsync();
                        }


                        var nuevoregistro = new PlanillaUsusarioProyecto
                        {
                            IdPlanilla = planilla.Id,
                            IdUsuProy = editarregistro.idusuproy,
                            RegistroHhProyecto = horasAsignadasDecimal,
                            FechaRegistro = editarregistro.Fecharegistro,
                            Observaciones = editarregistro.observaciones,
                            Idactividad = editarregistro.Idactividad,
                            Tipo = tipo,
                        };
                        context.PlanillaUsusarioProyectos.Add(nuevoregistro);
                        var usuarioproyecto = await context.UsuarioProyectos
                            .Include(up => up.IdUsuarioNavigation)
                             .ThenInclude(u => u.IdRecursoNavigation)
                                .FirstOrDefaultAsync(up => up.Id == editarregistro.idusuproy);
                        if (statusProyecto == 1)
                        {
                            await context.SaveChangesAsync();
                        }
                        if (statusProyecto == 2)
                        {
                            int idproyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == editarregistro.idusuproy).Select(up => up.IdProyectoNavigation.Id).FirstOrDefaultAsync();
                            var usuario = usuarioproyecto.IdUsuarioNavigation;
                            var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);

                            context.PlanillaRegistroEmpresas.Remove(registro);
                            await context.SaveChangesAsync();
                            await gestorhhplanilla(idproyecto, recurso.NombreRecurso, horasAsignadasDecimal, mesregistro, anioregistro, 0, editarregistro.proyectoorigen);
                        }
                        /* if (statusProyecto == 2)
                         {
                             if (usuarioproyecto != null)
                             {


                                 if (usuarioproyecto.HhConsultora.HasValue)
                                 {
                                     usuarioproyecto.HhConsultora -= horasAsignadasDecimal;
                                 }
                                 else if (usuarioproyecto.HhConsultorb.HasValue)
                                 {
                                     usuarioproyecto.HhConsultorb -= horasAsignadasDecimal;
                                 }
                                 else if (usuarioproyecto.HhConsultorc.HasValue)
                                 {
                                     usuarioproyecto.HhConsultorc -= horasAsignadasDecimal;
                                 }


                                 var usuario = usuarioproyecto.IdUsuarioNavigation;


                                 if (usuario != null && usuario.IdRecurso != 0)
                                 {
                                     var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                                     if (recurso != null)
                                     {

                                         if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                                         {

                                             if (recurso.NombreRecurso == "Socio")
                                             {


                                                 var usuariosRelacionados = await context.UsuarioProyectos
                                                 .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                                 .ToListAsync();


                                                 foreach (var usuarioRelacionado in usuariosRelacionados)
                                                 {
                                                     usuarioRelacionado.HhSocios -= horasAsignadasDecimal;
                                                 }
                                             }

                                             else if (recurso.NombreRecurso == "Staff")
                                             {


                                                 var usuariosRelacionados = await context.UsuarioProyectos
                                                 .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                                 .ToListAsync();


                                                 foreach (var usuarioRelacionado in usuariosRelacionados)
                                                 {
                                                     usuarioRelacionado.HhStaff -= horasAsignadasDecimal;
                                                 }
                                             }







                                         }
                                     }
                                 }

                                 else
                                 {
                                     Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                                     return 2;
                                 }
                             }

                         }*/

                        resultado = 1;
                    }

                }

                /*SUBACTIVIDAD ES 0*/

                else
                {
                    if (editarregistro.idsubactividad > 0 && (editarregistro.idusuproy == 0))
                    {
                        var registro = await context.PlanillaUsusarioProyectos.FindAsync(editarregistro.idregistro);
                        if (registro != null)
                        {
                            int empresaregistro = await RegistrarHorasEmpresa(editarregistro.idusuario, editarregistro.horasasignadas, editarregistro.Fecharegistro, editarregistro.observaciones, editarregistro.idsubactividad);
                            if (empresaregistro == 1)
                            {
                                decimal horasAsignadasDecimal;
                                if (!decimal.TryParse(editarregistro.horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                                {

                                    return 0;
                                }
                                int usuproy = await context.PlanillaUsusarioProyectos.Where(p => p.Id == editarregistro.idregistro).Select(p => p.IdUsuProy).FirstOrDefaultAsync();
                                if (usuproy != 0)
                                {

                                    var statusProyecto = await context.UsuarioProyectos
                                    .Where(up => up.Id == usuproy)
                                     .Join(context.Proyectos,
                                         up => up.IdProyecto,
                                            p => p.Id,
                                        (up, p) => p.StatusProyecto)
                                     .FirstOrDefaultAsync();
                                    if (statusProyecto == 1)
                                    {
                                        context.PlanillaUsusarioProyectos.Remove(registro);
                                        await context.SaveChangesAsync();

                                    }
                                    else if (statusProyecto == 2 || statusProyecto == 4)
                                    {
                                        int mesregistro = registro.FechaRegistro.Month;
                                        int anioregistro = registro.FechaRegistro.Year;
                                        var usuarioproyecto = await context.UsuarioProyectos
                                        .Include(up => up.IdUsuarioNavigation)
                                        .ThenInclude(u => u.IdRecursoNavigation)
                                        .FirstOrDefaultAsync(up => up.Id == usuproy);

                                        var usuario = usuarioproyecto.IdUsuarioNavigation;

                                        var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                                        int idproyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == registro.IdUsuProy).Select(up => up.IdProyectoNavigation.Id).FirstOrDefaultAsync();

                                        await gestorhhplanilla(idproyecto, recurso.NombreRecurso, horasAsignadasDecimal, mesregistro, anioregistro, 1, editarregistro.proyectoorigen);
                                        if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                                        {
                                            if (recurso.NombreRecurso == "Socio")
                                            {


                                                var usuariosRelacionados = await context.UsuarioProyectos
                                                .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                                .ToListAsync();


                                                foreach (var usuarioRelacionado in usuariosRelacionados)
                                                {
                                                    usuarioRelacionado.HhSocios += horasAsignadasDecimal;
                                                }
                                                context.PlanillaUsusarioProyectos.Remove(registro);
                                                await context.SaveChangesAsync();

                                            }
                                            else if (recurso.NombreRecurso == "Staff")
                                            {


                                                var usuariosRelacionados = await context.UsuarioProyectos
                                                .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                                .ToListAsync();


                                                foreach (var usuarioRelacionado in usuariosRelacionados)
                                                {
                                                    usuarioRelacionado.HhStaff += horasAsignadasDecimal;
                                                }
                                                context.PlanillaUsusarioProyectos.Remove(registro);
                                                await context.SaveChangesAsync();

                                            }
                                        }

                                    }


                                }

                                resultado = 1;
                            }
                            else
                            {
                                resultado = 2;
                            }

                            /* decimal? costoUnitario = await context.Recursos.Where(r => r.Usuarios.Any(u => u.Id == editarregistro.idusuario)).Select(r => r.CostoUnitario).FirstOrDefaultAsync();
                             decimal horasAsignadasDecimal;
                             if (!decimal.TryParse(editarregistro.horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                             {

                                 return 0;
                             }

                             int mesregistro = editarregistro.Fecharegistro.Month;
                             int anioregistro = editarregistro.Fecharegistro.Year;

                             var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == editarregistro.idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                             if (planilla == null)
                             {
                                 planilla = new Planilla
                                 {
                                     IdUsuario = editarregistro.idusuario,
                                     Mes = mesregistro,
                                     Anio = anioregistro,
                                 };
                                 context.Planillas.Add(planilla);
                                 await context.SaveChangesAsync();
                             }

                             var nuevoregistro = new PlanillaRegistroEmpresa
                             {
                                 IdPlanilla = planilla.Id,
                                 Hhregistradas = horasAsignadasDecimal,
                                 Fecharegistro = editarregistro.Fecharegistro,
                                 Observaciones = editarregistro.observaciones,
                                 Idsubactividad = editarregistro.idsubactividad,
                                 CostoMonetario = costoUnitario,
                                 Correlativo = GenerarCorrelativo(),
                             };
                             context.PlanillaRegistroEmpresas.Add(nuevoregistro);

                             await context.SaveChangesAsync();*/



                        }

                    }

                    /*EDITANDO PROYECTO CUANDO ANTES ERA PROYECTO*/
                    else if (editarregistro.idsubactividad == 0 && (editarregistro.idusuproy > 0))
                    {
                        var registro = await context.PlanillaUsusarioProyectos.FindAsync(editarregistro.idregistro);
                        if (registro != null)
                        {
                            int statusProyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == editarregistro.idusuproy).Select(up => up.IdProyectoNavigation.StatusProyecto).FirstOrDefaultAsync();
                            string tipo = string.Empty;

                            if (statusProyecto == 2)
                            {
                                tipo = "En Ejecución";
                            }
                            else if (statusProyecto == 1)
                            {
                                tipo = "En Negociación";
                            }
                            else
                            {
                                return 2;
                            }
                            decimal? horasiniciales = await context.PlanillaUsusarioProyectos.Where(r => r.Id == editarregistro.idregistro).Select(r => r.RegistroHhProyecto).FirstOrDefaultAsync();

                            decimal horasAsignadasDecimal;
                            if (!decimal.TryParse(editarregistro.horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                            {

                                return 0;
                            }

                            decimal? nuevashoras = horasAsignadasDecimal - horasiniciales;


                            int mesregistro = editarregistro.Fecharegistro.Month;
                            int anioregistro = editarregistro.Fecharegistro.Year;

                            var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == editarregistro.idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                            if (planilla == null)
                            {
                                planilla = new Planilla
                                {
                                    IdUsuario = editarregistro.idusuario,
                                    Mes = mesregistro,
                                    Anio = anioregistro,
                                };
                                context.Planillas.Add(planilla);
                                await context.SaveChangesAsync();
                            }
                            registro.IdPlanilla = planilla.Id;
                            registro.FechaRegistro = editarregistro.Fecharegistro;
                            registro.IdUsuProy = editarregistro.idusuproy;
                            registro.RegistroHhProyecto = horasAsignadasDecimal;
                            registro.Observaciones = editarregistro.observaciones;
                            registro.Idactividad = editarregistro.Idactividad;
                            registro.Tipo = tipo;

                            var usuarioproyecto = await context.UsuarioProyectos
                            .Include(up => up.IdUsuarioNavigation)
                             .ThenInclude(u => u.IdRecursoNavigation)
                                .FirstOrDefaultAsync(up => up.Id == editarregistro.idusuproy);
                            if (statusProyecto == 2)
                            {
                                int idproyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == editarregistro.idusuproy).Select(up => up.IdProyectoNavigation.Id).FirstOrDefaultAsync();
                                var usuario = usuarioproyecto.IdUsuarioNavigation;
                                var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                                await context.SaveChangesAsync();
                                await gestorhhplanilla(idproyecto, recurso.NombreRecurso, horasAsignadasDecimal, mesregistro, anioregistro, 2, editarregistro.proyectoorigen,editarregistro.Fechaorigen);
                            }
                            /*int usuproy = await context.PlanillaUsusarioProyectos.Where(p => p.Id == editarregistro.idregistro).Select(p => p.IdUsuProy).FirstOrDefaultAsync();
                            if (statusProyecto == 2)
                            {
                                var usuarioproyecto = await context.UsuarioProyectos
                                   .Include(up => up.IdUsuarioNavigation)
                                   .ThenInclude(u => u.IdRecursoNavigation)
                                   .FirstOrDefaultAsync(up => up.Id == usuproy);
                                if (usuarioproyecto != null)
                                {


                                    if (usuarioproyecto.HhConsultora.HasValue)
                                    {
                                        usuarioproyecto.HhConsultora -= nuevashoras;
                                    }
                                    else if (usuarioproyecto.HhConsultorb.HasValue)
                                    {
                                        usuarioproyecto.HhConsultorb -= nuevashoras;
                                    }
                                    else if (usuarioproyecto.HhConsultorc.HasValue)
                                    {
                                        usuarioproyecto.HhConsultorc -= nuevashoras;
                                    }


                                    var usuario = usuarioproyecto.IdUsuarioNavigation;


                                    if (usuario != null && usuario.IdRecurso != 0)
                                    {
                                        var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                                        if (recurso != null)
                                        {

                                            if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                                            {

                                                if (recurso.NombreRecurso == "Socio")
                                                {


                                                    var usuariosRelacionados = await context.UsuarioProyectos
                                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                                    .ToListAsync();


                                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                                    {
                                                        usuarioRelacionado.HhSocios -= nuevashoras;
                                                    }
                                                }

                                                else if (recurso.NombreRecurso == "Staff")
                                                {


                                                    var usuariosRelacionados = await context.UsuarioProyectos
                                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                                    .ToListAsync();


                                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                                    {
                                                        usuarioRelacionado.HhStaff -= nuevashoras;
                                                    }
                                                }


                                               




                                            }
                                        }
                                    }

                                    else
                                    {
                                        Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                                        return 2;
                                    }
                                }

                            }*/













                            resultado = 1;
                        }

                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al editar un registro:{ex.Message}");
                return 0;
            }
        }

        public async Task<bool> EliminarRegistro(int idregistro, string correlativo)
        {
            try
            {
                bool resultado = false;
                int idsubActividad = await context.PlanillaRegistroEmpresas
                .Where(p => p.Correlativo == correlativo)
                .Select(p => p.Idsubactividad)
                .FirstOrDefaultAsync();
                if (idsubActividad > 0)
                {
                    var registro = await context.PlanillaRegistroEmpresas.FindAsync(idregistro);
                    if (registro != null)
                    {
                        context.PlanillaRegistroEmpresas.Remove(registro);
                        await context.SaveChangesAsync();
                        resultado = true;
                    }
                }
                else
                {
                    var registro = await context.PlanillaUsusarioProyectos.FindAsync(idregistro);
                    int mesregistro = registro.FechaRegistro.Month;
                    int anioregistro = registro.FechaRegistro.Year;
                    decimal horasAsignadasDecimal = await context.PlanillaUsusarioProyectos.Where(p => p.Id == idregistro).Select(p => p.RegistroHhProyecto).FirstOrDefaultAsync() ?? 0;
                    int usuproy = await context.PlanillaUsusarioProyectos.Where(p => p.Id == idregistro).Select(p => p.IdUsuProy).FirstOrDefaultAsync();
                    if (usuproy != 0)
                    {

                        var statusProyecto = await context.UsuarioProyectos
                        .Where(up => up.Id == usuproy)
                         .Join(context.Proyectos,
                             up => up.IdProyecto,
                                p => p.Id,
                            (up, p) => p.StatusProyecto)
                         .FirstOrDefaultAsync();
                        if (statusProyecto == 1)
                        {
                            context.PlanillaUsusarioProyectos.Remove(registro);
                            await context.SaveChangesAsync();
                            resultado = true;
                        }
                        else if (statusProyecto == 2 || statusProyecto == 4)
                        {
                            var usuarioproyecto = await context.UsuarioProyectos
                            .Include(up => up.IdUsuarioNavigation)
                            .ThenInclude(u => u.IdRecursoNavigation)
                            .FirstOrDefaultAsync(up => up.Id == usuproy);

                            var usuario = usuarioproyecto.IdUsuarioNavigation;

                            var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                            int idproyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation).Where(up => up.Id == registro.IdUsuProy).Select(up => up.IdProyectoNavigation.Id).FirstOrDefaultAsync();

                            await gestorhhplanilla(idproyecto, recurso.NombreRecurso, horasAsignadasDecimal, mesregistro, anioregistro, 1);
                            if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                            {
                                if (recurso.NombreRecurso == "Socio")
                                {


                                    var usuariosRelacionados = await context.UsuarioProyectos
                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                    .ToListAsync();


                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                    {
                                        usuarioRelacionado.HhSocios += horasAsignadasDecimal;
                                    }
                                    context.PlanillaUsusarioProyectos.Remove(registro);
                                    await context.SaveChangesAsync();
                                    resultado = true;
                                }
                                else if (recurso.NombreRecurso == "Staff")
                                {


                                    var usuariosRelacionados = await context.UsuarioProyectos
                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                    .ToListAsync();


                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                    {
                                        usuarioRelacionado.HhStaff += horasAsignadasDecimal;
                                    }
                                    context.PlanillaUsusarioProyectos.Remove(registro);
                                    await context.SaveChangesAsync();
                                    resultado = true;
                                }
                            }

                        }


                    }
                    else
                    {
                        resultado = false;
                    }
                }

                return resultado;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al eliminar el registro:{e.Message}");
                return false;
            }
        }


        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaUsuario(int? idusuario, int? idplanilla)
        {
            try
            {
#pragma warning disable CS8600
                object idusuarioparameter = (object)idusuario ?? DBNull.Value;
                object idplanillaparameter = (object)idplanilla ?? DBNull.Value;

#pragma warning restore CS8600
                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "RECUPERARPLANILLAUSUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanillaparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),


                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),

                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                            };
                            planillausuario.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return planillausuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener la planilla del usuario:" + ex.Message);
                return new List<PlanillaUsuarioDTO>();

            }
        }


        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaEmpresaUsuario(int? idusuario, int? idplanilla)
        {
            try
            {
#pragma warning disable CS8600
                object idusuarioparameter = (object)idusuario ?? DBNull.Value;
                object idplanillaparameter = (object)idplanilla ?? DBNull.Value;

#pragma warning restore CS8600
                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "recuperarplanillaempresa";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanillaparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),


                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),

                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                            };
                            planillausuario.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return planillausuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener la planilla del usuario:" + ex.Message);
                return new List<PlanillaUsuarioDTO>();

            }
        }







        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaExcel(int idplanilla)
        {
            try
            {

                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "RECUPERARPLANILLAUSUARIO_EXCEL";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanilla));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                IdRegistro = reader.IsDBNull(reader.GetOrdinal("IdRegistro")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdRegistro")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("IdProyecto")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdProyecto")),
                                idproyectoantiguo = reader.IsDBNull(reader.GetOrdinal("IdProyectoantiguo")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdProyectoantiguo")),
                                IdActividad = reader.IsDBNull(reader.GetOrdinal("IdActividad")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdActividad")),
                                estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? 0 : reader.GetInt32(reader.GetOrdinal("estado")),
                                idusuproy = reader.IsDBNull(reader.GetOrdinal("idusuproy")) ? 0 : reader.GetInt32(reader.GetOrdinal("idusuproy")),
                                IDCUENTA = reader.IsDBNull(reader.GetOrdinal("idcuenta")) ? 0 : reader.GetInt32(reader.GetOrdinal("idcuenta")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("numproyecto")) ? null : reader.GetString(reader.GetOrdinal("numproyecto")),
                                Cuenta = reader.IsDBNull(reader.GetOrdinal("cuenta")) ? null : reader.GetString(reader.GetOrdinal("cuenta")),
                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? null : reader.GetString(reader.GetOrdinal("tipo")),
                                correlativo = reader.IsDBNull(reader.GetOrdinal("correlativo")) ? null : reader.GetString(reader.GetOrdinal("correlativo")),
                                ccosto = reader.IsDBNull(reader.GetOrdinal("ccosto")) ? null : reader.GetString(reader.GetOrdinal("ccosto")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                                NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                                Rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? string.Empty : reader.GetString(reader.GetOrdinal("Rol")),
                                CostoUnitario = reader.IsDBNull(reader.GetOrdinal("CostoUnitario")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CostoUnitario")),
                                CostoTotal = reader.IsDBNull(reader.GetOrdinal("CostoTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CostoTotal")),
                            };
                            planillausuario.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return planillausuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener la planilla excel del usuario:" + ex.Message);
                return new List<PlanillaUsuarioDTO>();

            }
        }



        public async Task<List<ProyectoGantt>> ObtenerDatosGantt(int idusuario)
        {
            var proyectosGantt = new List<ProyectoGantt>();
            var datosGantt = new List<DatosGanttDTO>();

            try
            {
                using (DbConnection connection = await conexion.OpenDatabaseConnectionAsync())
                {
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetHorasPorUsuarioPorMes";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdUsuario", idusuario));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var datos = new DatosGanttDTO
                                {
                                    NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NombreProyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreProyecto")),
                                    Año = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                    Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                    TotalHoras = reader.IsDBNull(reader.GetOrdinal("TotalHoras")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalHoras")),
                                    FechaInicio = reader.IsDBNull(reader.GetOrdinal("Fecha_Inicio")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Fecha_Inicio")),
                                    FechaTermino = reader.IsDBNull(reader.GetOrdinal("Fecha_Termino")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Fecha_Termino")),
                                };


                                datosGantt.Add(datos);
                            }
                        }
                    }
                }


                proyectosGantt = datosGantt
                    .GroupBy(d => d.NombreProyecto)
                    .Select(g => new ProyectoGantt
                    {
                        NombreProyecto = g.Key,
                        FechaInicio = g.First().FechaInicio,
                        FechaTermino = g.First().FechaTermino,
                        HorasPorMes = g.GroupBy(d => new { d.Año, d.Mes })
                                       .Select(m => new HorasPorMes
                                       {
                                           Año = m.Key.Año,
                                           Mes = m.Key.Mes,
                                           Horas = m.Sum(x => x.TotalHoras) // Sumar las horas totales por año y mes
                                       })
                                       .ToList()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los datos de la gantt: {ex.Message}");
                // Manejar la excepción según sea necesario
            }

            return proyectosGantt;
        }

        public async Task<List<HHUSUARIOPROYECTOTOTALDTO>> RecuperarHHUsuarios(int idusuario)
        {
            try
            {

                var proyectoshh = new List<HHUSUARIOPROYECTOTOTALDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "HH_USUARIO_PROYECTOS_GANTT";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HHUSUARIOPROYECTOTOTALDTO hhproyecto = new()
                            {
                                IDUSUARIOPROYCTO = reader.IsDBNull(reader.GetOrdinal("IDUSUARIOPROYCTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDUSUARIOPROYCTO")),
                                IDUSUARIO = reader.IsDBNull(reader.GetOrdinal("ID_USUARIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_USUARIO")),
                                Recurso = reader.IsDBNull(reader.GetOrdinal("RECURSO")) ? null : reader.GetString(reader.GetOrdinal("RECURSO")),
                                Nombre_Usuario = reader.IsDBNull(reader.GetOrdinal("NOMBRE_USUARIO")) ? null : reader.GetString(reader.GetOrdinal("NOMBRE_USUARIO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("ID_PROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_PROYECTO")),
                                HHRESTANTES = reader.IsDBNull(reader.GetOrdinal("HHASIGNADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHASIGNADAS")),
                                NOMBREPROYECTO = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),

                            };
                            proyectoshh.Add(hhproyecto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return proyectoshh;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los gantt:" + ex.Message);
                return new List<HHUSUARIOPROYECTOTOTALDTO>();

            }
        }



    }
}
