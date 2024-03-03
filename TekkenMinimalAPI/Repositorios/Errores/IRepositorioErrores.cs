using TekkenMinimalAPI.Entidades;

namespace TekkenMinimalAPI.Repositorios.Errores
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}