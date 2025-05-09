using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.CreateBipper
{
    public class CreateBipperCommandValidator : AbstractValidator<CreateBipperCommand>
    {
        public CreateBipperCommandValidator()
        {

            RuleFor(b => b.RestaurantId)
             .GreaterThan(0)
             .WithMessage("RestaurantId debe ser mayor a 0");

            RuleFor(b => b.ClientId)
                .GreaterThan(0)
                .WithMessage("ClientId debe ser mayor a 0");

            RuleFor(b => b.Type)
                .IsInEnum()
                .WithMessage("El tipo de Bipper es inválido")
                .NotNull();
                    

        }
    }
}
