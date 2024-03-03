﻿namespace TekkenMinimalAPI.DTO_s.Personajes
{
    public class CrearPersonajeDTO
    {
        public string Nombre { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public IFormFile? Foto { get; set; }
        public string Nacionalidad { get; set; } = null!;
        public Double Altura { get; set; }
        public Double Peso { get; set; }
        public string TipoDeSangre { get; set; } = null!;
    }
}
