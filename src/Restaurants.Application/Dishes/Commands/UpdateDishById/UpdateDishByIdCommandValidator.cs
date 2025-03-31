using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.UpdateDishById
{
    public class UpdateDishByIdCommandValidator : AbstractValidator<UpdateDishByIdCommand>
    {


        public UpdateDishByIdCommandValidator()
        {

            RuleFor(x => x.Name)
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.Description)
                .NotNull()
                .Length(3, 100);

            RuleFor(d => d.Price)
               .GreaterThanOrEqualTo(0)
               .WithMessage("The price must be a non-negative number");


            RuleFor(d => d.KiloCalories)
                 .GreaterThanOrEqualTo(0)
                .WithMessage("The KiloCalories must be a non-negative number");
        }
    }
    
}
