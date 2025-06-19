namespace Proyectogestionhoras.Models.ViewModel
{
    public class EditarRegistroViewModel
    {
        public int idregistro { get; set; }
        public int idusuario { get; set; }
        public int idusuproy { get; set; }
       
        public string? horasasignadas { get; set; }
        public string? correlativo { get; set; }
        public DateTime Fecharegistro { get; set; }
        public DateTime Fechaorigen { get; set; }
        public string? observaciones { get; set; }
        public int Idactividad { get; set; }
        public int idsubactividad { get; set; }
        public int proyectoorigen { get; set; }
    }
}
