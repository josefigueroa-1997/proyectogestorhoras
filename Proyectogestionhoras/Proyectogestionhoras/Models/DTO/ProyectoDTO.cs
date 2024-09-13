namespace Proyectogestionhoras.Models.DTO
{
    public class ProyectoDTO
    {
        public int Id { get; set; }
        public string? NombreProyecto { get; set; }
        public int Id_Cliente { get; set; }
        public string? Tipo_Status { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly Fecha_Termino { get; set; }
        public int Plazo { get; set; }  
        public string? Nombre_Cliente { get; set; }
        public string? Tipo_Tipologia { get; set; }
        public string? Tipo_Empresa {  get; set; }  
        public string? Ccosto { get; set; }
        public string? Linkedin { get;set; }
        public string? Integrantes_Proyecto { get; set; }
        public int HH_Semanales { get; set; }
        public string? Nombre_Recurso { get; set; }

    }
}
