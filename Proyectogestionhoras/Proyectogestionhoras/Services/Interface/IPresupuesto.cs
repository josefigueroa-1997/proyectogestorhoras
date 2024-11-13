namespace Proyectogestionhoras.Services.Interface
{
    public interface IPresupuesto
    {
        public Task<bool> EditarPresupuesto(int idproyecto, decimal monto, decimal costosocio, decimal costostaff, decimal costoconsultora, decimal costoconsultorb, decimal consultorc, int? hhsocio, int? hhstaff, int? hhconsultora, int? hhconsultorb, int? hhconsultorc);
       
    }
}
