using Microsoft.EntityFrameworkCore;
using TekkenMinimalAPI.DTO_s;
using TekkenMinimalAPI.Entidades;
using TekkenMinimalAPI.Utilidades;

namespace TekkenMinimalAPI.Repositorios.Personajes
{
    public class RepositorioPersonajes : IRepositorioPersonajes
    {
        private readonly ApplicationDBContext context;
        private readonly HttpContext httpContext;

        public RepositorioPersonajes(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Personaje>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Personajes.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            return await queryable
                .OrderBy(a => a.Nombre)
                .Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Personaje?> ObtenerPorId(int id)
        {
            return await context.Personajes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Personaje>> ObtenerPorNombre(string nombre)
        {
            return await context.Personajes
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(a => a.Nombre).ToListAsync();
        }

        public async Task<int> Crear(Personaje personaje)
        {
            context.Add(personaje);
            await context.SaveChangesAsync();
            return personaje.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Personajes.AnyAsync(p => p.Id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Personajes
                .Where(p => ids.Contains(p.Id))
                .Select(p => p.Id).ToListAsync();
        }

        public async Task Actualizar(Personaje personaje)
        {
            context.Update(personaje);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Personajes.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
