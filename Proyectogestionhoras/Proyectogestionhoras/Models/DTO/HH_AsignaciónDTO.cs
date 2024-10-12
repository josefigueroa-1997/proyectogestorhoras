namespace Proyectogestionhoras.Models.DTO
{
    public class HH_AsignaciónDTO
    {
        public int IDUSUARIOPROYECTO { get; set; }
        public int IDUSUARIO { get;set;}
        public string? RECURSO { get; set; }
        public string? NOMBREUSUARIO { get; set; }
        public int IDPROYECTO { get; set; } 
        public int HHASIGNADAS { get; set; }
        public string? TIPOCONSULTOR { get; set; }
    }
}
