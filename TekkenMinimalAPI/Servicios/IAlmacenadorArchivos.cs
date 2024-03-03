namespace TekkenMinimalAPI.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrrar(string? ruta, string contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);
        async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo)
        {
            await Borrrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }
    }
}
