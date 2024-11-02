namespace Proyectogestionhoras.Models.DTO
{
    public class HorasSociosActDTO
    {
        public string? NombreSocio { get; set; }
        public Dictionary<string,decimal>? HorasPorActividad { get; set; }

        public HorasSociosActDTO() { 
        
            HorasPorActividad = new Dictionary<string,decimal>();
        }
    }
}
