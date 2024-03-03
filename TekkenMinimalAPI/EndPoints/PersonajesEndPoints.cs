using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;
using TekkenMinimalAPI.DTO_s;
using TekkenMinimalAPI.DTO_s.Personajes;
using TekkenMinimalAPI.Entidades;
using TekkenMinimalAPI.Filtros;
using TekkenMinimalAPI.Repositorios.Personajes;
using TekkenMinimalAPI.Servicios;
using TekkenMinimalAPI.Utilidades;

namespace TekkenMinimalAPI.EndPoints
{
    public static class PersonajesEndPoints
    {
        private static readonly string contenedor = "Personajes";
        public static RouteGroupBuilder MapPersonajes(this RouteGroupBuilder group) 
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                .Tag("personajes-get"))
                .AgregarParametrosPaginacionAOpenAPI();

            group.MapGet("/{id:int}", ObtenerPorId);

            group.MapGet("obtenerPorNombre/{nombre}", ObtenerPorNombre);

            group.MapPost("/", Crear)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPersonajeDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapPut("/{id:int}", Actualizar)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPersonajeDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization("esadmin");

            return group;
        }

        static async Task<Results<Ok<List<PersonajeDTO>>, NotFound>> ObtenerTodos(IRepositorioPersonajes repositorio, IMapper mapper,
            PaginacionDTO paginacion)
        {
            var personajes = await repositorio.ObtenerTodos(paginacion);

            var personajesDTO = mapper.Map<List<PersonajeDTO>>(personajes);

            if (personajesDTO.Count == 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(personajesDTO);
        }

        static async Task<Results<Ok<PersonajeDTO>, NotFound>> ObtenerPorId(int id,
            IRepositorioPersonajes repositorio, IMapper mapper)
        {
            var personaje = await repositorio.ObtenerPorId(id);

            if (personaje is null)
            {
                return TypedResults.NotFound();
            }

            var personajeDTO = mapper.Map<PersonajeDTO>(personaje);

            return TypedResults.Ok(personajeDTO);
        }

        static async Task<Ok<List<PersonajeDTO>>> ObtenerPorNombre(string nombre,
            IRepositorioPersonajes repositorio, IMapper mapper)
        {
            var personaje = await repositorio.ObtenerPorNombre(nombre);

            var personajeDTO = mapper.Map<List<PersonajeDTO>>(personaje);

            return TypedResults.Ok(personajeDTO);
        }

        static async Task<Results<Created<PersonajeDTO>, ValidationProblem>> Crear([FromForm] CrearPersonajeDTO crearPersonajeDTO,
            IRepositorioPersonajes repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var personaje = mapper.Map<Personaje>(crearPersonajeDTO);

            if (crearPersonajeDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPersonajeDTO.Foto);
                personaje.Foto = url;
            }

            var id = await repositorio.Crear(personaje);

            await outputCacheStore.EvictByTagAsync("personajes-get", default);

            var personajeDTO = mapper.Map<PersonajeDTO>(personaje);

            return TypedResults.Created($"/personajes/{id}", personajeDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPersonajeDTO crearPersonajeDTO,
            IRepositorioPersonajes repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var personajeBD = await repositorio.ObtenerPorId(id);

            if (personajeBD is null)
            {
                return TypedResults.NotFound();
            }

            var personajeParaActualizar = mapper.Map<Personaje>(crearPersonajeDTO);
            personajeParaActualizar.Id = id;
            personajeParaActualizar.Foto = personajeBD.Foto;

            if (crearPersonajeDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Editar(personajeParaActualizar.Foto, contenedor, crearPersonajeDTO.Foto);
                personajeParaActualizar.Foto = url;
            }

            await repositorio.Actualizar(personajeParaActualizar);
            await outputCacheStore.EvictByTagAsync("personajes-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPersonajes repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var personajeBD = await repositorio.ObtenerPorId(id);

            if (personajeBD is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrrar(personajeBD.Foto, contenedor);
            await outputCacheStore.EvictByTagAsync("personajes-get", default);

            return TypedResults.NoContent();
        }
    }
}
