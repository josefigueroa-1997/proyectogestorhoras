using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class MetasController : Controller
    {

        private readonly MetaService _metaService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public MetasController(MetaService metaService,PROYECTO_CONTROL_HORASContext context) { 
        
            this._metaService = metaService;
            this.context = context;
        
        }


        public IActionResult MetasFunciones()
        {
            return View();
        }
        public async Task<IActionResult> ParametrosMetas()
        {
            var datos = await _metaService.GetMetaFacturacionqx();
            var tipologias = await _metaService.GetMetasTipologias();
            ViewBag.DatosFacturas = datos;
            ViewBag.Tipologias = tipologias;
            return View();
        }

        public IActionResult SalidasMetas()
        {
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> RegistrarMetaFacturas()
        {
            int anio = int.Parse(Request.Form["anio"].ToString());
            decimal q1 = decimal.Parse(Request.Form["q1"].ToString());
            decimal q2 = decimal.Parse(Request.Form["q2"].ToString());
            decimal q3 = decimal.Parse(Request.Form["q3"].ToString());
            decimal q4 = decimal.Parse(Request.Form["q4"].ToString());

            bool resultado = await _metaService.RegistrarMetaFacturacion(anio,q1, q2, q3, q4);
            if (resultado)
            {
                return RedirectToAction("ParametrosMetas","Metas");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarMetaTipologia()
        {
            int anio = int.Parse(Request.Form["aniotipologia"].ToString());
            int t1 = int.Parse(Request.Form["t1"].ToString());
            int t2 = int.Parse(Request.Form["t2"].ToString());
            int t3 = int.Parse(Request.Form["t3"].ToString());
            int t4 = int.Parse(Request.Form["t4"].ToString());
            int totalproyecto = int.Parse(Request.Form["totals"].ToString());
            decimal totalporproyecto = decimal.Parse(Request.Form["totalproyecto"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            int duracion = int.Parse(Request.Form["duracion"].ToString());
            decimal monto = decimal.Parse(Request.Form["montomensual"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

            bool resultado = await _metaService.RegistrarMetaTipologia(anio,t1,t2,t3,t4,totalproyecto, totalporproyecto, duracion,monto);
            if (resultado) {

                return RedirectToAction("ParametrosMetas", "Metas");
            
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDatosMetas(int anio)
        {
            var resultado = await _metaService.GetDatosMeta(anio);
            return Json(resultado);
        }
        [HttpGet]
        public async Task<IActionResult> GetFacturasQuarter(int anio)
        {
            var resultado = await _metaService.GetDatosMetafacturaq(anio);
            return Json(resultado);
        }
    }
}
