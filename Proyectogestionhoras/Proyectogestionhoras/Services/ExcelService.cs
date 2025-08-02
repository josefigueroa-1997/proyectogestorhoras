using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


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

            foreach (var egreso in egresos)
            {
                var idProyecto = egreso.Idpeoyecto;
                var idEgreso = egreso.Idegreso;
                var glosa = egreso.Observacion?.Trim() ?? "";

                if (egreso.Tiposervicio != "Gastos")
                {
                    servicios.Add(new Serviciosejecucion
                    {
                        Idproyecto = idProyecto,
                        Idservicio = idEgreso,
                        Idproveedor = egreso.Idproveedor,
                        Fecha = egreso.Fecha,
                        Monto = egreso.Monto,
                        Observacion = glosa,
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
                        Observacion = glosa,
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
            try
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
                            int idcuenta = await context
                            .Servicios
                            .Where(s => s.Id == servicio.Idservicios)
                            .Select(s => s.Idcuenta.Value)
                            .FirstOrDefaultAsync();

                            int? idsegmento = await context.Segmentos
                            .Where(s => s.IdCuenta == idcuenta && s.TipoSegmento == "Servicios")
                            .Select(s => (int?)s.Id) 
                                .FirstOrDefaultAsync();

                            if (idsegmento == null)
                            {
                                idsegmento = 65;
                            }

                            serviciosAGrabar.Add(new ProyectoServicio
                            {
                                IdProyecto = idProyecto,
                                IdServicio = servicio.Idservicios,
                                Idsegmento = idsegmento,
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
            catch (Exception ex)
            {

                Debug.WriteLine("Error al agregar servicios al proyecto: " + ex.Message, ex);
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
                        int idcuenta = await context
                      .Gastos
                      .Where(s => s.Id == gasto.Idgastos)
                      .Select(s => s.Idcuenta.Value)
                      .FirstOrDefaultAsync();




                        var segmento = await context.Segmentos
                        .FirstOrDefaultAsync(s => s.IdCuenta == idcuenta && s.TipoSegmento == "Gastos");

                        int idsegmento = segmento?.Id ?? 96;


                        gastosAGrabar.Add(new ProyectoGasto
                        {
                            IdProyecto = idProyecto,
                            IdGastos = gasto.Idgastos,
                            Idsegmento = idsegmento,
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
