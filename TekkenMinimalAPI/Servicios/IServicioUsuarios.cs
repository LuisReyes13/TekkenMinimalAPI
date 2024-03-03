using Microsoft.AspNetCore.Identity;

namespace TekkenMinimalAPI.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}