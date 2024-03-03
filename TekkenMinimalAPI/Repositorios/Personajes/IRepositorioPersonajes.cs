using TekkenMinimalAPI.DTO_s;
using TekkenMinimalAPI.Entidades;

namespace TekkenMinimalAPI.Repositorios.Personajes
{
    public interface IRepositorioPersonajes
    {
        Task Actualizar(Personaje personaje);
        Task Borrar(int id);
        Task<int> Crear(Personaje personaje);
        Task<bool> Existe(int id);
        Task<List<int>> Existen(List<int> ids);
        Task<Personaje?> ObtenerPorId(int id);
        Task<List<Personaje>> ObtenerPorNombre(string nombre);
        Task<List<Personaje>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}