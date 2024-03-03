using TekkenMinimalAPI.Entidades;

namespace TekkenMinimalAPI.Repositorios.Errores
{
    public class RepositorioErrores : IRepositorioErrores
    {
        private readonly ApplicationDBContext context;

        public RepositorioErrores(ApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task Crear(Error error)
        {
            context.Add(error);
            await context.SaveChangesAsync();
        }
    }
}
