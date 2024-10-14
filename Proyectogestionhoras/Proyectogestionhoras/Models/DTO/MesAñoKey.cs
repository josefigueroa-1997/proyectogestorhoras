namespace Proyectogestionhoras.Models.DTO
{
    public class MesAñoKey
    {
        public int Año { get; set; }
        public int Mes { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MesAñoKey key &&
                   Año == key.Año &&
                   Mes == key.Mes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Año, Mes);
        }
    }
}
