using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class MetasController : Controller
    {

        private readonly MetaService _metaService;
        public MetasController(MetaService metaService) { 
        
            this._metaService = metaService;
        
        }


        public IActionResult MetasFunciones()
        {
            return View();
        }
        public async Task<IActionResult> ParametrosMetas()
        {
            var datos = await _metaService.GetMetaFacturacionqx();
            ViewBag.DatosFacturas = datos;
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
    }
}
