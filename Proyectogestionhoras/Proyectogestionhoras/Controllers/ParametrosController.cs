﻿using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using System.Diagnostics;

namespace Proyectogestionhoras.Controllers
{
    public class ParametrosController : Controller
    {
        private readonly PROYECTO_CONTROL_HORASContext context;

        public ParametrosController(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }

        public IActionResult TodoslosParametros()
        {

            return View("Parametros");
        }

        /*Cuentas*/
        public async Task<IActionResult> GestorCuentas() 
        {

            var cuentas = await ObtenerCuentas();
            ViewBag.Cuentas = cuentas;
            return View();
        
        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarCuenta(Cuentum cuentas)
        {
            if (cuentas == null)
            {
                return BadRequest("La cuenta es nula.");
            }

           
            if (cuentas.Id == 0)
            {
              
                context.Cuenta.Add(cuentas);
            }
            else
            {
               
                var cuentaExistente = await context.Cuenta.FindAsync(cuentas.Id);
                if (cuentaExistente == null)
                {
                    return NotFound("Cuenta no encontrada.");
                }
               
                cuentaExistente.Cuenta = cuentas.Cuenta; 
                context.Cuenta.Update(cuentaExistente);
            }

            
            await context.SaveChangesAsync();
            return RedirectToAction("GestorCuentas");
        }

        public async Task<List<Cuentum>> ObtenerCuentas()
        {
            var cuentas = await context.Cuenta.ToListAsync();
            return cuentas;
        }

        /*Gastos*/
        public async Task<IActionResult> GestorGastos()
        {

            var gastos = await context.Gastos.ToListAsync();
            ViewBag.Gastos = gastos;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarGasto(Gasto gastos)
        {
            if (gastos == null)
            {
                return BadRequest("el Gasto es nulo.");
            }


            if (gastos.Id == 0)
            {

                context.Gastos.Add(gastos);
            }
            else
            {

                var gastoexistente = await context.Gastos.FindAsync(gastos.Id);
                if (gastoexistente == null)
                {
                    return NotFound("Gasto no encontrado.");
                }

                gastoexistente.Nombre = gastos.Nombre;
                context.Gastos.Update(gastoexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorGastos");
        }
        /*Servicios*/
        public async Task<IActionResult> GestorServicios()
        {

            var servicios = await context.Servicios.ToListAsync();
            ViewBag.Servicios = servicios;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> GuardarActualizarServicio(Servicio servicios)
        {
            if (servicios == null)
            {
                return BadRequest("el Serivicio es nulo.");
            }


            if (servicios.Id == 0)
            {

                context.Servicios.Add(servicios);
            }
            else
            {

                var servicioexistente = await context.Servicios.FindAsync(servicios.Id);
                if (servicioexistente == null)
                {
                    return NotFound("Servicio no encontrado.");
                }

                servicioexistente.Nombre = servicios.Nombre;
                context.Servicios.Update(servicioexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorServicios");
        }
        /*Actividades*/
        public async Task<IActionResult> GestorActividades()
        {

            var actividades = await context.Actividades.ToListAsync();
            ViewBag.Actividades = actividades;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarActividades(Actividade actividades)
        {
            if (actividades == null)
            {
                return BadRequest("La Actividad es nula.");
            }


            if (actividades.Id == 0)
            {

                context.Actividades.Add(actividades);
            }
            else
            {

                var actividadexistente = await context.Actividades.FindAsync(actividades.Id);
                if (actividadexistente == null)
                {
                    return NotFound("Actividad no encontrada.");
                }

                actividadexistente.Nombre = actividades.Nombre;
                actividadexistente.TipoAcatividad = actividades.TipoAcatividad;
                context.Actividades.Update(actividadexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorActividades");
        }

        /*Segmentos*/
        public async Task<IActionResult> GestorSegmentos()
        {
            var query = from un in context.CcostoUnegocios
                        join sc in context.SegmentoCcostos on un.Id equals sc.IdCcosto
                        join s in context.Segmentos on sc.IdSegmento equals s.Id
                        join c in context.Cuenta on s.IdCuenta equals c.Id
                        select new
                        {   
                            idsegmento = s.Id,
                            tiposegmento = s.TipoSegmento,
                            SegmentoNombre = s.Nombre,
                            UnidadCodigo = un.Codigo,
                            CuentaId = c.Idcuenta,
                            CuentaNombre = c.Cuenta,
                            idcuenta = s.IdCuenta,
                            Idunidad = un.Id,
                        };

            var segmentos = await query.ToListAsync();
            var cuentas = await context.Cuenta.ToListAsync();
            var ccostos = await context.CcostoUnegocios.ToListAsync();
            ViewBag.Ccostos = ccostos;
            ViewBag.Cuentas = cuentas;
            ViewBag.Segmentos = segmentos;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> GuardarActualizarSegmentos(Segmento segmentos,SegmentoCcosto segmentoCcostos)
        {
            try
            {
                if (segmentos == null || segmentoCcostos == null)
                {
                    return BadRequest("La Actividad es nula.");
                }



                if (segmentos.Id == 0)
                {

                    context.Segmentos.Add(segmentos);
                    await context.SaveChangesAsync();
                    segmentoCcostos.IdSegmento = segmentos.Id;
                    segmentoCcostos.IdCcosto = int.Parse(Request.Form["Idccosto"].ToString());
                    context.SegmentoCcostos.Add(segmentoCcostos);
                    await context.SaveChangesAsync();
                }
                else
                {

                    var segmentoexistente = await context.Segmentos.FindAsync(segmentos.Id);
                    
                    if (segmentoexistente == null)
                    {
                        return NotFound("Segmento no encontrado.");
                    }

                    segmentoexistente.Nombre = segmentos.Nombre;
                    segmentoexistente.TipoSegmento = segmentos.TipoSegmento;
                    segmentoexistente.IdCuenta = segmentos.IdCuenta;
                    context.Segmentos.Update(segmentoexistente);
                    await context.SaveChangesAsync();

                   



                }



                return RedirectToAction("GestorSegmentos");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar el segmento:{ex.Message}");
                return View();
            }
         }

        /*Tipologias*/
        public async Task<IActionResult> GestorTipologias()
        {

            var tipologias = await context.Tipologia
                              .OrderByDescending(t => t.Id)
                              .ToListAsync();
            ViewBag.Tipologias = tipologias;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarTipologias(Tipologium tipologias)
        {
            if (tipologias == null)
            {
                return BadRequest("La tipología es nula.");
            }


            if (tipologias.Id == 0)
            {

                context.Tipologia.Add(tipologias);
            }
            else
            {

                var tipologiaexistente = await context.Tipologia.FindAsync(tipologias.Id);
                if (tipologiaexistente == null)
                {
                    return NotFound("tipología no encontrada.");
                }
                decimal monto = decimal.Parse(Request.Form["Monto"].ToString());

                tipologiaexistente.TipoTipologia = tipologias.TipoTipologia;
                tipologiaexistente.Monto = monto;
                tipologiaexistente.Desde = tipologias.Desde;
                tipologiaexistente.Hasta = tipologias.Hasta;
                tipologiaexistente.Hhsocios = tipologias.Hhsocios;
                tipologiaexistente.Hhstaff = tipologias.Hhstaff;
                tipologiaexistente.Consultora = tipologias.Consultora;
                tipologiaexistente.Consutlrob = tipologias.Consutlrob;
                tipologiaexistente.Consultorc = tipologias.Consultorc;
                context.Tipologia.Update(tipologiaexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorTipologias");
        }

    }
}