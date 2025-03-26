using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant
{
    public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
    {
        public UpdateRestaurantCommandValidator()
        {

            RuleFor(r => r.Name)
                .Length(3, 100)
                .NotEmpty()
                .WithMessage("Name cannot be empty");

            RuleFor(r => r.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty");

        }
    }
}
