using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;

namespace Proyectogestionhoras.Services.Interface
{
    public interface IMeta
    {
        public Task<bool> RegistrarMetaFacturacion(int anio, decimal q1,decimal q2,decimal q3,decimal q4);
        public Task<List<MetaFacturacionqxDTO>> GetMetaFacturacionqx();
        public Task<bool> RegistrarMetaTipologia(int anio,int t1,int t2,int t3,int t4,int totalproyecto,decimal totalporproyecto,int duracion,decimal montomensual);
        public Task<List<MetasTipologiaDTO>> GetMetasTipologias();
    }
}
