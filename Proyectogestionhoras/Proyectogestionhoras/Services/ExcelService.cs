using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Proyectogestionhoras.Services
{
    public class ExcelService : IExcel
    {
        private readonly PROYECTO_CONTROL_HORASContext context;

        public ExcelService(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }

        public async Task IngresarEgresosMasivosExcel(List<EgresosExcelViewModel> egresos)
        {
            if (egresos == null || !egresos.Any())
                return;

            var servicios = new List<Serviciosejecucion>();
            var gastos = new List<Gastosejecucion>();

            
            var glosaPorProyecto = new Dictionary<int, Dictionary<string, List<int>>>();

            foreach (var egreso in egresos)
            {
                var idProyecto = egreso.Idpeoyecto;
                var idEgreso = egreso.Idegreso;
                var glosaOriginal = egreso.Observacion?.Trim() ?? "";
                string glosaFinal = glosaOriginal;

                if (!glosaPorProyecto.ContainsKey(idProyecto))
                    glosaPorProyecto[idProyecto] = new Dictionary<string, List<int>>();

                var glosasProyecto = glosaPorProyecto[idProyecto];

                if (!glosasProyecto.ContainsKey(glosaOriginal))
                {
                    glosasProyecto[glosaOriginal] = new List<int> { idEgreso };
                }
                else
                {
                    var egresosUsados = glosasProyecto[glosaOriginal];

                    if (!egresosUsados.Contains(idEgreso))
                    {
                        int repeticion = egresosUsados.Count;
                        glosaFinal = $"{glosaOriginal}_{repeticion}";
                        egresosUsados.Add(idEgreso);
                    }
                    
                }

                if (egreso.Tiposervicio != "Gastos")
                {
                    servicios.Add(new Serviciosejecucion
                    {
                        Idproyecto = idProyecto,
                        Idservicio = idEgreso,
                        Idproveedor = egreso.Idproveedor,
                        Fecha = egreso.Fecha,
                        Monto = egreso.Monto,
                        Observacion = glosaFinal,
                        Estado = egreso.Estado,
                        Venta = "Vendido",
                        Tiposervicio = egreso.Tiposervicio
                    });
                }
                else
                {
                    gastos.Add(new Gastosejecucion
                    {
                        Idproyecto = idProyecto,
                        Idgasto = idEgreso,
                        Idproveedor = egreso.Idproveedor,
                        Fecha = egreso.Fecha,
                        Monto = egreso.Monto,
                        Observacion = glosaFinal,
                        Estado = egreso.Estado,
                        Venta = "Vendido"
                    });
                }
            }

            if (servicios.Any())
                await context.Serviciosejecucions.AddRangeAsync(servicios);

            if (gastos.Any())
                await context.Gastosejecucions.AddRangeAsync(gastos);

            await context.SaveChangesAsync();
        }


        public async Task RegistrarIngresosMasivosExcel(List<IngresosExcelViewModel> ingresos)
        {
            if (ingresos == null || !ingresos.Any())
                return;

            var listaingreso = new List<Ingresosreale>();

            foreach (var ingreso in ingresos)
            {
              
                
                    listaingreso.Add(new Ingresosreale
                    {
                        Idproyecto = ingreso.Idproyecto,
                        Numdocumento = ingreso.numdocumento,
                        FechaEmision = ingreso.Fechaemision,
                        FechaPago = ingreso.Fechapago,
                        Montoclp = ingreso.Monto,
                        Iva = ingreso.iva,
                        Estado = ingreso.Estado,
                        Venta = "Vendido",
                        Observacion = ingreso.Observacion,
                        Montous = 0,
                        Tc = 0,
                        Idcuenta = 4101001,

                    });
                
            }


            if (listaingreso.Any())
                await context.Ingresosreales.AddRangeAsync(listaingreso);
            await context.SaveChangesAsync();

        }
        public async Task AgregarServicioProyectonuevos(List<ServicioViewModel> servicios)
        {
            if (servicios == null || !servicios.Any())
            {
                return;
            }

            var todosProyectos = servicios
                .SelectMany(s => s.IdsProyecto)
                .Distinct()
                .ToList();

            
            var serviciosExistentes = await context.ProyectoServicios
                .Where(ps => todosProyectos.Contains(ps.IdProyecto))
                .Select(ps => new { ps.IdProyecto, ps.IdServicio })
                .ToListAsync();

            var yaAgregados = new HashSet<(int, int)>(); 
            var serviciosAGrabar = new List<ProyectoServicio>();

            foreach (var servicio in servicios)
            {
                foreach (var idProyecto in servicio.IdsProyecto)
                {
                    var clave = (idProyecto, servicio.Idservicios);

                    bool yaExisteBD = serviciosExistentes.Any(se =>
                        se.IdProyecto == idProyecto &&
                        se.IdServicio == servicio.Idservicios);

                    if (!yaExisteBD && !yaAgregados.Contains(clave))
                    {
                        yaAgregados.Add(clave);

                        serviciosAGrabar.Add(new ProyectoServicio
                        {
                            IdProyecto = idProyecto,
                            IdServicio = servicio.Idservicios,
                            Idsegmento = servicio.IdSegmento,
                            Monto = 0,
                            Fecha = DateTime.Now
                        });
                    }
                }
            }

            if (serviciosAGrabar.Any())
            {
                await context.ProyectoServicios.AddRangeAsync(serviciosAGrabar);
                await context.SaveChangesAsync();
            }
        }

        public async Task AgregarGastosProyectonuevos(List<GastoViewModel> gastos)
        {
            if (gastos == null || !gastos.Any())
            {
                return;
            }

            var todosProyectos = gastos
                .SelectMany(s => s.IdProyecto)
                .Distinct()
                .ToList();


            var gastosExistentes = await context.ProyectoGastos
                .Where(ps => todosProyectos.Contains(ps.IdProyecto))
                .Select(ps => new { ps.IdProyecto, ps.IdGastos })
                .ToListAsync();

            var yaAgregados = new HashSet<(int, int)>();
            var gastosAGrabar = new List<ProyectoGasto>();

            foreach (var gasto in gastos)
            {
                foreach (var idProyecto in gasto.IdProyecto)
                {
                    var clave = (idProyecto, gasto.Idgastos);

                    bool yaExisteBD = gastosExistentes.Any(se =>
                        se.IdProyecto == idProyecto &&
                        se.IdGastos == gasto.Idgastos);

                    if (!yaExisteBD && !yaAgregados.Contains(clave))
                    {
                        yaAgregados.Add(clave);

                        gastosAGrabar.Add(new ProyectoGasto
                        {
                            IdProyecto = idProyecto,
                            IdGastos = gasto.Idgastos,
                            Idsegmento = gasto.IdSegmento,
                            Monto = 0,
                            Fecha = DateTime.Now
                        });
                    }
                }
            }

            if (gastosAGrabar.Any())
            {
                await context.ProyectoGastos.AddRangeAsync(gastosAGrabar);
                await context.SaveChangesAsync();
            }
        }


    }
}
