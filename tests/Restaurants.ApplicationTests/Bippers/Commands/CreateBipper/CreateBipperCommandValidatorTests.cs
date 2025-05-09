using Xunit;
using Restaurants.Application.Bippers.Commands.CreateBipper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurants.Domain.Constants;
using FluentAssertions;
using FluentValidation.TestHelper;


namespace Restaurants.Application.Bippers.Commands.CreateBipper.Tests
{
    public class CreateBipperCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
        {

            //arrange

            var command = new CreateBipperCommand
            {
                RestaurantId = 1,
                ClientId = 1,
                IsReady = true,
                Type = BipperType.TableAssignment,
                isActive = true
            };


            var validator = new CreateBipperCommandValidator();


            //act

            var result = validator.TestValidate(command);


            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact()]
        public void Validator_ForInvalidCommand_ShouldHaveValidationErrors()
        {

            //arrange

            var command = new CreateBipperCommand
            {
                RestaurantId = 0,
                ClientId = 0,
                IsReady = false,
                isActive = true
            };


            var validator = new CreateBipperCommandValidator();


            //act

            var result = validator.TestValidate(command);


            //assert

            result.ShouldHaveValidationErrorFor(b => b.RestaurantId);
            result.ShouldHaveValidationErrorFor(b => b.ClientId);
        }
    }
}