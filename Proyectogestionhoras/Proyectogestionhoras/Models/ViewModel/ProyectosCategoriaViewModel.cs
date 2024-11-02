namespace Proyectogestionhoras.Models.ViewModel
{
    public class ProyectosCategoriaViewModel
    {
        public IEnumerable<Tipologium>? Tipologias { get; set; }
        public IEnumerable<Ccosto>? Ccosto { get; set; }
        public IEnumerable<Unegocio>? Unegocio { get; set; }
        public IEnumerable<CcostoUnegocio>? CcostoUnegocios { get; set; }
  
    }
}
