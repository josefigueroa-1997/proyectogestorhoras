using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;
using OfficeOpenXml;
using Proyectogestionhoras.Models.DTO;
using Newtonsoft.Json;
namespace Proyectogestionhoras.Controllers
{
    public class ExcelController : Controller
    {
        private readonly ReporteService reporteService;
        public ExcelController(ReporteService reporteService) 
        { 
            this.reporteService = reporteService;
        }
        public IActionResult Index()
        {
            return View();
        }

    }

}



