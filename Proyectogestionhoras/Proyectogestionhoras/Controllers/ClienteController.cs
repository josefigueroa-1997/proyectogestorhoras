﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly ContactoService contactoService;

        public ClienteController(ClienteService service, ProyectoService proyectoService,ContactoService contactoService)
        {
            this.service = service;
            this.proyectoService = proyectoService;
            this.contactoService = contactoService;
        }

        public async Task <IActionResult> NuestrosClientes(int? id)
        {
            var clientes = await service.ObtenerClientesIndex(id);
            ViewBag.Clientes = clientes;
            return View();
        }

        public async Task <IActionResult> ProyectosCliente(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {

            var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre,idtipoempresa, statusproyecto,numproyecto,idtipologia,unidadneg,idccosto, idusuario);
            var clientedetalle = await service.ObtenerClientesIndex(idcliente);
            var contactos = await contactoService.ObtenerContactos(0, idcliente, null);
            if (idcliente.HasValue)
            {
                HttpContext.Session.SetInt32("IdCliente", idcliente.Value);
            }
            ViewBag.Proyectos = proyectos;
            ViewBag.Contactos = contactos;
            ViewBag.clientedetalle = clientedetalle;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCliente(string nombre,string? direccion,string? ciudad,string? pais,string? telefono,string? pagweb,string? linkedin,string? instagram,string idcliente, string sucursal,string tipocliente)
        {
            try
            {

                var idusuario = HttpContext.Session.GetInt32("id");
                bool registro = await service.RegistrarCliente(nombre, direccion, ciudad, pais, telefono, pagweb, linkedin, instagram,idcliente,sucursal, tipocliente);
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
        [HttpPost]
        public async Task<IActionResult> AgregarSucursal(int idcli, string nombre)
        {
            try
            {
                bool resultado = await service.AgregarSucursal(idcli, nombre);
                if (resultado)
                {

                    return RedirectToAction("ProyectosCliente", new { idcliente = idcli });

                }
                else { 
                
                    return View();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al agregar una sucursal:"+ex.Message);
                return View();
            }


        }
        public async Task<IActionResult> EditarCliente(int id)
        {
            {
                var cliente = await service.ObtenerClientesIndex(id);
                var sucursales = await service.ObtenerSucursal(id);
                ViewBag.clientedatos = cliente;
                ViewBag.Sucursales = sucursales;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditarCliente(int id, string nombre, string direccion, string ciudad, string pais, string telefono, string? pagweb, string? linkedin, string? instagram, int idsucursal, string nombresucursal,string tipocliente)
        {
            try
            {
                bool resultado = await service.EditarCliente(id,nombre,direccion, ciudad,pais, telefono,pagweb, linkedin, instagram,idsucursal, nombresucursal, tipocliente);
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

        [HttpGet]
        public async Task<IActionResult> ObtenerSucursales(int idcliente)
        {
            var sucursales = await service.ObtenerSucursal(idcliente);
            return Json(sucursales);
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExistenciaIdCliente(string idcliente)
        {
            int verificar = await service.ValidarExistenciaIdCliente(idcliente);
            return Ok(verificar);
        }

    }

}
