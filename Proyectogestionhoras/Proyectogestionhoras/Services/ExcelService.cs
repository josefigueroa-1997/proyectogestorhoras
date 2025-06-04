using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;

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
                if (egreso.Tiposervicio != "Gastos")
                {
                    servicios.Add(new Serviciosejecucion
                    {
                        Idproyecto = egreso.Idpeoyecto,
                        Idservicio = egreso.Idegreso,
                        Idproveedor = egreso.Idproveedor,
                        Fecha = egreso.Fecha,
                        Monto = egreso.Monto,
                        Observacion = egreso.Observacion,
                        Estado = egreso.Estado,
                        Venta = egreso.Venta,
                        Tiposervicio = egreso.Tiposervicio
                    });
                }
                else
                {
                    gastos.Add(new Gastosejecucion
                    {
                        Idproyecto = egreso.Idpeoyecto,
                        Idgasto = egreso.Idegreso,
                        Idproveedor = egreso.Idproveedor,
                        Fecha = egreso.Fecha,
                        Monto = egreso.Monto,
                        Observacion = egreso.Observacion,
                        Estado = egreso.Estado,
                        Venta = egreso.Venta
                    });
                }
            }

            
            if (servicios.Any())
                await context.Serviciosejecucions.AddRangeAsync(servicios);

            if (gastos.Any())
                await context.Gastosejecucions.AddRangeAsync(gastos);

            await context.SaveChangesAsync(); 
        }


    }
}
