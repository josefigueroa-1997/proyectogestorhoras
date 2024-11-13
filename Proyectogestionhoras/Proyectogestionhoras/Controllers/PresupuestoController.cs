using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class PresupuestoController : Controller
    {
        private readonly ProyectoService _proyectoService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly PresupuestoService presupuestoService;

        public PresupuestoController(ProyectoService proyectoService, PROYECTO_CONTROL_HORASContext context,PresupuestoService presupuestoService)
        {
            _proyectoService = proyectoService;
            this.context = context;
            this.presupuestoService = presupuestoService;
        }

        public async Task<IActionResult> EditarPresupuesto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await _proyectoService.ObtenerProyectos(id,idcliente, nombre, idtipoempresa,statusproyecto,numproyecto,idtipologia,unidadneg,idccosto,idusuario);
            var serviciosproyecto = await _proyectoService.ObtenerServiciosProyecto(id);
            var gastosproyecto = await _proyectoService.ObtenerGastosProyectos(id);
            var servicios = GetServicios();
            var gastos =  GetGastos();
            ViewBag.GastosProyectos = gastosproyecto;
            ViewBag.ServiciosProyectos = serviciosproyecto;
            ViewBag.Servicios = servicios;
            ViewBag.Gastos = gastos;
            ViewBag.Proyectos = proyecto;
            return View();
        }

        public async Task<IActionResult> ActualizarPresupuesto(int idproyecto)
        {
            var montopresupuesto = Request.Form["monto"].ToString();
            var montopresupuestostr = montopresupuesto.Replace(".", "");
            decimal montofinal = decimal.Parse(montopresupuestostr);
            
            var costosocios = Request.Form["costosocio"].ToString();
            var costosociostr = costosocios.Replace(".", "");

            decimal costosociofinal = decimal.Parse(costosociostr);

            var costostaff = Request.Form["costostaff"].ToString();
            var costostaffstr = costostaff.Replace(".", "");

            decimal costostafffinal = decimal.Parse(costostaffstr);

            var costoconsultora = Request.Form["costoconsultora"].ToString();
            var costoconsultorastr = costoconsultora.Replace(".", "");

            decimal costoconsultorafinal = decimal.Parse(costoconsultorastr);

            var costoconsultorb = Request.Form["costoconsultorb"].ToString();
            var costoconsultorbstr = costoconsultorb.Replace(".", "");
            decimal costoconsultorbfinal = decimal.Parse(costoconsultorbstr);

            var costoconsultorc = Request.Form["costoconsultorc"].ToString();
            var costoconsultorcstr = costoconsultorc.Replace(".", "");
            decimal costoconsultorcfinal = decimal.Parse(costoconsultorcstr);


            List<ServicioViewModel> servicios = new List<ServicioViewModel>();
            var idsservicios = Request.Form["idservicio"];
            var idsegmentoservicio = Request.Form["idsegmentoservicio"];
            var montoservicioList = Request.Form["montoservicio"];
            var fechaservicio = Request.Form["fechaservicio"];
            var esEliminados = Request.Form["esEliminado"];


            
            List<GastoViewModel> gastos = new List<GastoViewModel>();
            var idgastos = Request.Form["idgastos[]"];
            var idsegmentogasto = Request.Form["idsegmentogasto"];
            var montogastoList = Request.Form["montogasto"];
            var fechagasto = Request.Form["fechagasto"];
            var esEliminadogastos = Request.Form["esEliminados"];

            for (int i = 0; i < idsservicios.Count; i++)
            {
                var montoservicioStr = montoservicioList[i].ToString();

                montoservicioStr = montoservicioStr.Replace(".", "");

                decimal montoservicio = decimal.Parse(montoservicioStr);
                var servicioViewModel = new ServicioViewModel
                {
                    Idservicios = int.Parse(idsservicios[i]),
                    IdSegmento = int.Parse(idsegmentoservicio[i]),
                    MontoServicio = montoservicio,
                    Fecha = DateTime.Parse(fechaservicio[i]),
                    EsEliminado = esEliminados[i] == "true",

                };

                servicios.Add(servicioViewModel);
            }


            for (int i = 0; i < idgastos.Count; i++)
            {
                var montogastoStr = montogastoList[i].ToString();

                montogastoStr = montogastoStr.Replace(".", "");

                decimal montogasto = decimal.Parse(montogastoStr);
                var gasto = new GastoViewModel
                {
                    Idgastos = int.Parse(idgastos[i]),
                    IdSegmento = int.Parse(idsegmentogasto[i]),
                    MontoGasto = montogasto,
                    Fecha = DateTime.Parse(fechagasto[i]),
                    EsEliminado = esEliminadogastos[i] == "true",

                };

                gastos.Add(gasto);
            }
            int idpresupuesto = int.Parse(Request.Form["idpresupuesto"].ToString());
            await _proyectoService.GestorServiciosProyecto(idproyecto, servicios);
            await _proyectoService.GestorProyectoGastos(idproyecto, gastos);
            bool resultado = await presupuestoService.EditarPresupuesto(idproyecto, montofinal, costosociofinal, costostafffinal, costoconsultorafinal, costoconsultorbfinal, costoconsultorcfinal, 0, 0, 0,0,0);
            if(resultado == true)
            {
                return RedirectToAction("ObtenerPresupuestoProyecto", "Proyecto", new {idpresupuesto= idpresupuesto,id=idproyecto});
            }
            else
            {
                return View();
            }
        }

        public List<Servicio> GetServicios()
        {
            var resultado = context.Servicios
            .ToList();
            return resultado;
        }

        public List<Gasto> GetGastos()
        {
            var resultado =context.Gastos.ToList();
            return resultado;
        }
    }
}
