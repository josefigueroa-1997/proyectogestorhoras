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
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");
   
        }
        public async Task<IActionResult> ParametrosMetas()
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        var datos = await _metaService.GetMetaFacturacionqx(0);
                        var tipologias = await _metaService.GetMetasTipologias(0);
                        ViewBag.DatosFacturas = datos;
                        ViewBag.Tipologias = tipologias;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");
            
        }
        public async Task<IActionResult> EditarMetaFactura(int id)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        var datosmetafactura = await _metaService.GetMetaFacturacionqx(id);

                        return View(datosmetafactura);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");
            
        }

        public async Task<IActionResult> EditarMetaTipologia(int id)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        var datosmetatipologia = await _metaService.GetMetasTipologias(id);
                        return View(datosmetatipologia);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");
            
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarMetaFactura()
        {
            int id = int.Parse(Request.Form["id"].ToString());
            int anio = int.Parse(Request.Form["anio"].ToString());
            decimal q1 = decimal.Parse(Request.Form["montoQ1"].ToString());
            decimal q2 = decimal.Parse(Request.Form["montoQ2"].ToString());
            decimal q3 = decimal.Parse(Request.Form["montoQ3"].ToString());
            decimal q4 = decimal.Parse(Request.Form["montoQ4"].ToString());
            bool resultado = await _metaService.ActualizarMetaFactura(id,anio,q1,q2,q3,q4);
            if (resultado)
            {
                return RedirectToAction("ParametrosMetas", "Metas");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarMetaTipologia()
        {
            int id = int.Parse(Request.Form["id"].ToString());
            int anio = int.Parse(Request.Form["anio"].ToString());
            int t1 = int.Parse(Request.Form["t1"].ToString());
            int t2 = int.Parse(Request.Form["t2"].ToString());
            int t3 = int.Parse(Request.Form["t3"].ToString());
            int t4 = int.Parse(Request.Form["t4"].ToString());
            bool resultado = await _metaService.ActualizarMetaTipologia(id,anio,t1,t2,t3,t4);
            if (resultado)
            {
                return RedirectToAction("ParametrosMetas", "Metas");
            }
            else
            {
                return View();
            }
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

            string totalporproyectoStr = Request.Form["totalproyecto"].ToString();
            decimal totalporproyecto = ConvertToDecimal(totalporproyectoStr);

            int duracion = int.Parse(Request.Form["duracion"].ToString());

    
            string montoStr = Request.Form["montomensual"].ToString();
            decimal monto = ConvertToDecimal(montoStr);

            bool resultado = await _metaService.RegistrarMetaTipologia(anio, t1, t2, t3, t4, totalproyecto, totalporproyecto, duracion, monto);
            if (resultado)
            {
                return RedirectToAction("ParametrosMetas", "Metas");
            }
            else
            {
                return View();
            }
        }

        private decimal ConvertToDecimal(string input)
        {

            input = input.Replace(".", "").Replace(",", ".");

     
            return decimal.Parse(input, System.Globalization.CultureInfo.InvariantCulture);
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


        [HttpGet]
        public IActionResult ValidaddispFactura(int anio)
        {
            var resultado = VerificarDisponibilidadFactura(anio);
            return Json(resultado);
        }
        [HttpGet]
        public IActionResult Validaddisptipo(int anio)
        {
            var resultado = VerificarDisponibilidadtipologia(anio);
            return Json(resultado);
        }

        public int VerificarDisponibilidadFactura(int anio)
        {
            int cantidad = context.MetaFacturacionesqxes
            .Where(m => m.Anio == anio) 
            .Count();
            return cantidad;
        }

        public int VerificarDisponibilidadtipologia(int anio)
        {
            int cantidad = context.Metatipologias
            .Where(m => m.Anio == anio)
            .Count();
            return cantidad;
        }


    }
}
