using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;

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
    }
}
