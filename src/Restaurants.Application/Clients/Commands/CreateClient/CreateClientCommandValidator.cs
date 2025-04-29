using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Clients.Commands.CreateClient
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {

            RuleFor(c => c.Nombre)
                .NotEmpty()
                .NotNull()
                .WithMessage("El nombre no puede estar vacío");

            RuleFor(c => c.Apellido)
                .NotEmpty()
                .NotNull()
                .WithMessage("El Apellido no puede estar vacío");

            RuleFor(c => c.Email)
                .EmailAddress()
                .WithMessage("Please provide a valid email address");

            RuleFor(c => c.Phone)
                .NotEmpty()
                .NotNull()
                .WithMessage("El telefono no puede estar vacío");

        }
    }
}
