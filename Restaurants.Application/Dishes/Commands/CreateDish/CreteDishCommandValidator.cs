using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.CreateDish
{
    public class CreteDishCommandValidator : AbstractValidator<CreateDishCommand>
    {
        public CreteDishCommandValidator()
        {


            RuleFor(d => d.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The price must be a non-negative number");


            RuleFor(d => d.KiloCalories)
                 .GreaterThanOrEqualTo(0)
                .WithMessage("The KiloCalories must be a non-negative number");
        }
    }
}
