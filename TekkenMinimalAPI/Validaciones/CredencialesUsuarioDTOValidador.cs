using FluentValidation;
using TekkenMinimalAPI.DTO_s;

namespace TekkenMinimalAPI.Validaciones
{
    public class CredencialesUsuarioDTOValidador : AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidador()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilidades.MaximumLengthMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailMensaje);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }
    }
}
