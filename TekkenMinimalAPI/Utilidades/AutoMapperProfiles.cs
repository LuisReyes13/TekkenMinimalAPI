using AutoMapper;
using TekkenMinimalAPI.DTO_s.Personajes;
using TekkenMinimalAPI.Entidades;

namespace TekkenMinimalAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearPersonajeDTO, Personaje>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Personaje, PersonajeDTO>();
        }
    }
}
