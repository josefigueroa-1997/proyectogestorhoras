namespace Proyectogestionhoras.Models.DTO
{
    public class HorasSociosActDTO
    {
        public string? NombreSocio { get; set; }
        public Dictionary<string,int>? HorasPorActividad { get; set; }

        public HorasSociosActDTO() { 
        
            HorasPorActividad = new Dictionary<string,int>();
        }
    }
}
