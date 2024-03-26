using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;
using TekkenMinimalAPI.DTO_s;
using TekkenMinimalAPI.DTO_s.Personajes;
using TekkenMinimalAPI.Entidades;
using TekkenMinimalAPI.Filtros;
using TekkenMinimalAPI.Migrations;
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
                .AgregarParametrosPaginacionAOpenAPI()
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Obtener todos los personajes";
                    opciones.Description = "Con este endpoint se obtienen todos los personajes";

                    return opciones;
                });

            group.MapGet("/{id:int}", ObtenerPorId)
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Obtener personaje por Id";
                    opciones.Description = "Con este endpoint se obtiene un personaje por su Id";
                    opciones.Parameters[0].Description = "El Id del personaje a obtener";

                    return opciones;
                }); 

            group.MapGet("obtenerPorNombre/{nombre}", ObtenerPorNombre)
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Obtener personaje por Nombre";
                    opciones.Description = "Con este endpoint se obtiene un personaje por su nombre";
                    opciones.Parameters[0].Description = "El nombre del personaje a obtener";

                    return opciones;
                }); 

            group.MapPost("/", Crear)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPersonajeDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Crear personaje";
                    opciones.Description = "Con este endpoint se crea un personaje";

                    return opciones;
                });

            group.MapPut("/{id:int}", Actualizar)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPersonajeDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Actualizar un personaje";
                    opciones.Description = "Con este endpoint se puede actualizar un personaje";
                    opciones.Parameters[0].Description = "El Id del personaje a actualizar";
                    opciones.RequestBody.Description = "El personaje que se desea actualizar";
                    return opciones;
                });

            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization("esadmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Borrar personaje";
                    opciones.Description = "Con este endpoint se borra un personaje";

                    return opciones;
                }); 

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

        static async Task<Results<Created<PersonajeDTO>, ValidationProblem, Conflict<string>>> Crear([FromForm] CrearPersonajeDTO crearPersonajeDTO,
            IRepositorioPersonajes repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var personaje = mapper.Map<Personaje>(crearPersonajeDTO);

            var existeNombre = await repositorio.Existe(personaje.Nombre);

            if (existeNombre)
            {
                return TypedResults.Conflict("Ya existe un registro con el mismo nombre");
            }

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

        static async Task<Results<NoContent, NotFound, Conflict<string>>> Actualizar(int id, [FromForm] CrearPersonajeDTO crearPersonajeDTO,
            IRepositorioPersonajes repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var personajeBD = await repositorio.ObtenerPorId(id);

            if (personajeBD is null)
            {
                return TypedResults.NotFound();
            }

            //var existeNombre = await repositorio.Existe(personajeBD.Nombre);

            //if (existeNombre)
            //{
            //    return TypedResults.Conflict("Ya existe un registro con el mismo nombre");
            //}

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
