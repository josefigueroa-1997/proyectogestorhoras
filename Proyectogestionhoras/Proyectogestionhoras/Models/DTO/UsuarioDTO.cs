﻿namespace Proyectogestionhoras.Models.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Nombre_Usuario { get; set; }
        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public string? Nombre_Recurso { get; set; }
        public int Numero_Horas { get; set; }
        public decimal Costo_Unitario { get; set; }
        public string? Rol { get; set; }




    }
}