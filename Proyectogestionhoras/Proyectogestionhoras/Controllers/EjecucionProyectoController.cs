﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;
using System.Diagnostics;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Controllers
{
    public class EjecucionProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly FacturaService facturaService;
        private readonly EjecucionService ejecucionService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public EjecucionProyectoController(ProyectoService proyectoService, FacturaService facturaService, EjecucionService ejecucionService, PROYECTO_CONTROL_HORASContext context)
        {
            this.proyectoService = proyectoService;
            this.facturaService = facturaService;
            this.ejecucionService = ejecucionService;
            this.context = context;
        }

        public async Task<IActionResult> SeleccionarProyecto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            ViewBag.Proyectos = proyectos;
            return View();
        }

        [HttpPost]
        public IActionResult RedirigirActividad()
        {
            int idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            HttpContext.Session.SetInt32("numproyecto", idproyecto);
            return RedirectToAction("SeleccionarActividad", "EjecucionProyecto");
        }
        public IActionResult SeleccionarActividad() {

            return View();
        }

        /*FORECAST INGRESOS*/
        public async Task<IActionResult> ForecastIngreso(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var factura = await facturaService.RecuperarFacturas(id);
            var cuotas = await context.Ingresosreales.Where(x => x.Idproyecto == id).ToListAsync();
            ViewBag.Cuotas = cuotas;
            ViewBag.Proyecto = proyecto;
            ViewBag.Factura = factura;
            return View();
        }

      

        [HttpPost]
        public async Task<IActionResult> RegistrarIngresos(int idproyecto, List<IngresoViewModel> ingresos)
        {
            if (ingresos == null || !ingresos.Any())
            {
                ModelState.AddModelError("", "Debe ingresar al menos una factura.");
                return View(); 
            }

            await ejecucionService.GestorIngresos(idproyecto, ingresos);
            return RedirectToAction("ForecastIngreso", "EjecucionProyecto", new {id = idproyecto}); 
        }

        /*FORECAST COSTOS*/
        public async Task<IActionResult> ForecastCostos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var serviciosejecucion = await context.Serviciosejecucions.Where(s => s.Idproyecto == id).ToListAsync();
            var gastosejecucion = await ejecucionService.ObtenerGastosReales(id);
            var servicios = await GetServicios();
            var gastos = await GetGastos();
            var proveedoresservicios = await GetProveedoresServicios();
            var proveedoresgastos = await GetProveedoresGastos();
            var gastoshh = await ejecucionService.ObtenerGastosHH(id);
            ViewBag.Proyecto = proyecto;
            ViewBag.ServiciosEjecucion = serviciosejecucion;
            ViewBag.GastosEjecucion = gastosejecucion;
            ViewBag.Servicios = servicios;
            ViewBag.Gastos = gastos;
            ViewBag.Proveedores = proveedoresservicios;
            ViewBag.ProGastos = proveedoresgastos;
            ViewBag.GastosHH = gastoshh;
            return View();
        }


        public async Task<List<Servicio>> GetServicios()
        {
            var resultado = await context.Servicios
            .ToListAsync();
            return resultado;
        }
        public async Task<List<Gasto>> GetGastos()
        {
            var resultado = await context.Gastos
            .ToListAsync();
            return resultado;
        }

        public async Task<List<Proveedore>> GetProveedoresServicios()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Servicio%"))
            .ToListAsync();
            return proveedores;
        }
        public async Task<List<Proveedore>> GetProveedoresGastos()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Gasto%"))
            .ToListAsync();
            return proveedores;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCostos(int idproyecto)
        {
            /*COSTOS SERVICIOS REALES*/
            List<ServiciosRealesViewModel> servicios = new List<ServiciosRealesViewModel>();
            var idsservicios = Request.Form["Idservicio"];
            var idproveedores = Request.Form["Idproveedor"];
            var montoservicioList = Request.Form["montoservicio"];
            var fechaservicio = Request.Form["fechaservicio"];
            var idservicioreal = Request.Form["IdServicioReal"];
           
            for (int i = 0; i < idsservicios.Count; i++)
            {
                var montoservicioStr = montoservicioList[i]?.ToString().Trim() ?? ""; 
                if (string.IsNullOrEmpty(montoservicioStr))
                {
                    montoservicioStr = "0"; 
                }
                else
                {
                    montoservicioStr = montoservicioStr.Replace(".", ""); 
                }

                decimal montoservicio = decimal.Parse(montoservicioStr);

                int idServicioRealParsed = string.IsNullOrWhiteSpace(idservicioreal[i])
                                           ? 0
                                           : int.Parse(idservicioreal[i]);

                DateTime fechaServicioParsed;
                if (string.IsNullOrWhiteSpace(fechaservicio[i]))
                {
                    fechaServicioParsed = DateTime.Today;  
                }
                else
                {
                    fechaServicioParsed = DateTime.Parse(fechaservicio[i]);  
                }
                var servicioViewModel = new ServiciosRealesViewModel
                {
                    IdServicioReal = idServicioRealParsed, 
                    Idservicio = int.Parse(idsservicios[i]),
                    Idproveedor = int.Parse(idproveedores[i]),
                    Monto = montoservicio,
                    Fecha = fechaServicioParsed,
                };

                servicios.Add(servicioViewModel);
            }


            /*Gastos*/
            List<GastosRealesViewModel> gastos = new List<GastosRealesViewModel>();
            var idsgastos = Request.Form["idgastos"];
            var idproveedoresgastos = Request.Form["idproveedorgastos"];
            var idsegmentogastos = Request.Form["idsegmentogasto"];
            var montogastosList = Request.Form["montogasto"];
            var fechagastos = Request.Form["fechagasto"];
            var idgastoreal = Request.Form["IdGastoReal"];
            
            
            
            for (int i = 0; i < idsgastos.Count; i++)
            {
                var montogastoStr = montogastosList[i]?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(montogastoStr))
                {
                    montogastoStr = "0";
                }
                else
                {
                    montogastoStr = montogastoStr.Replace(".", "");
                }

                decimal montogasto = decimal.Parse(montogastoStr);

                int idgastoRealParsed = string.IsNullOrWhiteSpace(idgastoreal[i])
                                           ? 0
                                           : int.Parse(idgastoreal[i]);

                DateTime fechaGastoParsed;
                if (string.IsNullOrWhiteSpace(fechagastos[i]))
                {
                    fechaGastoParsed = DateTime.Today;
                }
                else
                {
                    fechaGastoParsed = DateTime.Parse(fechagastos[i]);
                }
                var gastoViewModel = new GastosRealesViewModel
                {
                    IdGastoReal = idgastoRealParsed,
                    Idgasto = int.Parse(idsgastos[i]),
                    Idproveedor = int.Parse(idproveedoresgastos[i]),
                    Segmento = int.Parse(idsegmentogastos[i]),
                    Monto = montogasto,
                    Fecha = fechaGastoParsed,
                };

                gastos.Add(gastoViewModel);


            }
            

            await ejecucionService.GestorServiciosReales(idproyecto, servicios);
            await ejecucionService.GestorGastosReales(idproyecto, gastos);
            return RedirectToAction("ForecastCostos", "EjecucionProyecto", new {id = idproyecto });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresServicios()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Servicio%"))
            .ToListAsync();
            return Json(proveedores);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresGastos()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Gasto%"))
            .ToListAsync();
            return Json(proveedores);
        }
    }
}