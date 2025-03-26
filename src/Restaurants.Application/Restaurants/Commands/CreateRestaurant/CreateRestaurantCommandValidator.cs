using FluentValidation;
using Restaurants.Application.Restaurants.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant
{
    public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
    {

        private readonly List<string> ValidCategories = ["Italian", "Mexican", "Japanese", "American", "Indian"];

        public CreateRestaurantCommandValidator()
        {

            RuleFor(dto => dto.Name)
                .Length(3, 100);

            RuleFor(dto => dto.Category)
                .Must(category => ValidCategories.Contains(category!)) //u can even simplify this by passing this .Must(ValidCategories.Contains)
                .WithMessage("Invalid Category. Please choose from the valid categories");

                //.Custom((value, context) =>
                //{
                //    var isValidCategory = ValidCategories.Contains(value!);

                //    if (!isValidCategory)
                //    {
                //        context.AddFailure("Category", "Invalid Category. Please choose from the valid categories");
                //    }


                //});



            RuleFor(dto => dto.ContactEmail)
                .EmailAddress()
                .WithMessage("Please provide a valid email address");

            RuleFor(dto => dto.PostalCode)
                .Matches(@"^\d{2}-\d{3}$")
                .WithMessage("Please provida a valid postal code (XX-XXX)");

        }
    }
}
