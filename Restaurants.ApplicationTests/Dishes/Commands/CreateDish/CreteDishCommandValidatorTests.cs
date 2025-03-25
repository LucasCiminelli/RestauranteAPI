using Xunit;
using Restaurants.Application.Dishes.Commands.CreateDish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Restaurants.Application.Dishes.Commands.CreateDish.Tests
{
    public class CreteDishCommandValidatorTests
    {
        [Fact()]
        public void CreteDishCommandValidatorTest()
        {
            var dish = new CreateDishCommand()
            {
                Name = "Test Dish",
                Description = "Test description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var commandValidator = new CreteDishCommandValidator();

            var result = commandValidator.TestValidate(dish);


            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact()]
        public void Validator_ForValidCommand_ShouldHaveValidationErrors()
        {

            var dish = new CreateDishCommand()
            {
                Name = "Test Dish",
                Description = "Test description",
                Price = -10,
                KiloCalories = -100
            };

            var commandValidator = new CreteDishCommandValidator();

            var result = commandValidator.TestValidate(dish);


            result.ShouldHaveValidationErrorFor(d => d.Price);
            result.ShouldHaveValidationErrorFor(d => d.KiloCalories);

        }
    }
}