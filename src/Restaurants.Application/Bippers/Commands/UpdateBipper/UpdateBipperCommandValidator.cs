using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.UpdateBipper
{
    public class UpdateBipperCommandValidator : AbstractValidator<UpdateBipperCommand>
    {
        public UpdateBipperCommandValidator()
        {

            RuleFor(b => b.Status)
                .IsInEnum()
                .WithMessage("The status is invalid");

            RuleFor(b => b.Type)
                .IsInEnum()
                .WithMessage("The type is invalid");

        }
    }
}
