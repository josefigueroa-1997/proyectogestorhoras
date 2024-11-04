using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IBono
    {
        public Task<List<BonoDTO>> RecuperarDatosBonos(int anio);
    }
}
