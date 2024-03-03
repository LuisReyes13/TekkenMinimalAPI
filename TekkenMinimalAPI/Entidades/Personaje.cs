using Microsoft.EntityFrameworkCore;

namespace TekkenMinimalAPI.Entidades
{
    public class Personaje
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Foto { get; set; }
        public string Nacionalidad { get; set; } = null!;
        public Double Altura { get; set; }
        public Double Peso { get; set; }
        public string TipoDeSangre { get; set; } = null!;
    }
}
