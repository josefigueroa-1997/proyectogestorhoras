using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using System.Diagnostics;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Services;
using Microsoft.AspNetCore.Http;
using Proyectogestionhoras.Services.Interface;
namespace Proyectogestionhoras.Controllers
{
    public class ClienteController : Controller
    {

        private readonly ClienteService service;
        private readonly ProyectoService proyectoService;

        public ClienteController(ClienteService service, ProyectoService proyectoService)
        {
            this.service = service;
            this.proyectoService = proyectoService;
        }

        public async Task <IActionResult> NuestrosClientes(int? id)
        {
            var clientes = await service.ObtenerClientesIndex(id);
            ViewBag.Clientes = clientes;
            return View();
        }

        public async Task <IActionResult> ProyectosCliente(int? id, int? idcliente, int? idusuario, string? nombre, int? idtipoempresa, string? statusproyecto)
        {

            var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, idusuario, nombre,idtipoempresa, statusproyecto);
            var clientedetalle = await service.ObtenerClientesIndex(idcliente);
            
            ViewBag.clientedetalle = clientedetalle;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCliente(string nombre,string direccion,string ciudad,string pais,string telefono,string? pagweb,string? linkedin,string? instagram)
        {
            try
            {

                
                bool registro = await service.RegistrarCliente(nombre, direccion, ciudad, pais, telefono, pagweb, linkedin, instagram);
                if (registro) {
                    TempData["SuccessMessage"] = "¡Se Agregó con éxito el nuevo cliente!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Debug.Write($"Hubo un error en el registro del cliente");
                    return View();
                }
               
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar el cliente"+ex.Message);
                return View();
            }
        }
        public async Task<IActionResult> EditarCliente(int id)
        {
            {
                var cliente = await service.ObtenerClientesIndex(id);
                ViewBag.clientedatos = cliente;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditarCliente(int id, string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram)
        {
            try
            {
                bool resultado = await service.EditarCliente(id,nombre,direccion, ciudad,pais, telefono,pagweb, linkedin, instagram);
                if (resultado) {
                    TempData["SuccessMessage"] = "¡Se Edito con éxito la información del cliente!";
                    return RedirectToAction("ProyectosCliente", "Cliente", new {idcliente=id});
                }
                else
                {
                    return View();
                }
            
            }
            catch(Exception ex)
            {
                Debug.Write($"Hubo un error en la edición"+ex);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            try
            {
                bool resultado = await service.EliminarCliente(id);
                if (resultado) {

                    return RedirectToAction("Index", "Home", new {id=0});
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Hubo un error al eliminar al cliente:" + ex.Message); 
                return View();
            }
        }

    }

}
