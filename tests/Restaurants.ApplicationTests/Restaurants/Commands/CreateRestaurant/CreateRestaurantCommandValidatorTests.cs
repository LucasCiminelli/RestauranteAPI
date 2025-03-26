using Xunit;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests
{
    public class CreateRestaurantCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
        {


            //Arrange

            var command = new CreateRestaurantCommand()
            {
                Name = "Test",
                Category = "Italian",
                ContactEmail = "test@test.com",
                PostalCode = "12-345"
            };


            var validator = new CreateRestaurantCommandValidator();

            //act

            var result = validator.TestValidate(command);

            //assert

            result.ShouldNotHaveAnyValidationErrors();



        }

        [Fact()]
        public void Validator_ForValidCommand_ShouldHaveValidationErrors()
        {


            //Arrange

            var command = new CreateRestaurantCommand()
            {
                Name = "Te",
                Category = "Argentinian",
                ContactEmail = "@test.com",
                PostalCode = "12345"
            };


            var validator = new CreateRestaurantCommandValidator();

            //act

            var result = validator.TestValidate(command);

            //assert

            result.ShouldHaveValidationErrorFor(c => c.Name);
            result.ShouldHaveValidationErrorFor(c => c.Category);
            result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
            result.ShouldHaveValidationErrorFor(c => c.PostalCode);
        }

        [Theory()]
        [InlineData("Italian")]
        [InlineData("Mexican")]
        [InlineData("Japanese")]
        [InlineData("American")]
        [InlineData("Indian")]


        public void Validator_ForValidCategory_ShouldNotHaveValidationErrorsForCategoryProperty(string category)
        {

            //arrange 

            var validator = new CreateRestaurantCommandValidator();

            var command = new CreateRestaurantCommand { Category = category };


            //act

            var result = validator.TestValidate(command);


            //assert 

            result.ShouldNotHaveValidationErrorFor(c => c.Category);
        }


        [Theory()]
        [InlineData("12020")]
        [InlineData("102-20")]
        [InlineData("10 220")]
        [InlineData("10-2 20")]
        [InlineData("20 2-10")]

        public void Validator_ForInvalidPostalCode_ShouldHaveValidationErrorsForPostalCodeProperty(string postalCode)
        {

            //arrange

            var validator = new CreateRestaurantCommandValidator();

            var command = new CreateRestaurantCommand { PostalCode = postalCode };

            //act

            var result = validator.TestValidate(command);

            //assert

            result.ShouldHaveValidationErrorFor(c => c.PostalCode);
        }

    }
}