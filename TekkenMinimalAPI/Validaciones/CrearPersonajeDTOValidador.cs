using FluentValidation;
using TekkenMinimalAPI.DTO_s.Personajes;

namespace TekkenMinimalAPI.Validaciones
{
    public class CrearPersonajeDTOValidador : AbstractValidator<CrearPersonajeDTO>
    {
        public CrearPersonajeDTOValidador()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(30).WithMessage(Utilidades.MaximumLengthMensaje)
                .Must(Utilidades.PrimeraLetraEnMayusculas).WithMessage(Utilidades.PrimeraLetraMayusculaMensaje);

            RuleFor(x => x.NombreCompleto)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.MaximumLengthMensaje);

            RuleFor(x => x.TipoDeSangre)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(30).WithMessage(Utilidades.MaximumLengthMensaje);

            RuleFor(x => x.Altura)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);

            RuleFor(x => x.Peso)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }
    }
}
