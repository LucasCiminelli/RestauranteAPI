using Xunit;
using Restaurants.Application.Restaurants.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;

namespace Restaurants.Application.Restaurants.Dtos.Tests
{
    public class RestaurantsProfileTests
    {
        private readonly IMapper _mapper;

        public RestaurantsProfileTests()
        {

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RestaurantsProfile>();
            });

            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact()]
        public void CreateMap_ForRestaurantToRestaurantDTO_MapsCorrectly()
        {

            var restaurant = new Restaurant()
            {
                Id = 1,
                Name = "Test restaurant",
                Description = "Test Description",
                Category = "Test Category",
                HasDelivery = true,
                ContactEmail = "test@example.com",
                ContactNumber = "123456789",
                Address = new Address
                {
                    City = "Test City",
                    Street = "Test Street",
                    PostalCode = "12-345"
                }
            };

            //act

            var restaurantDTO = _mapper.Map<RestaurantDTO>(restaurant);


            //assert

            restaurantDTO.Should().NotBeNull();
            restaurantDTO.Id.Should().Be(restaurant.Id);
            restaurantDTO.Name.Should().Be(restaurant.Name);
            restaurantDTO.Description.Should().Be(restaurant.Description);
            restaurantDTO.Category.Should().Be(restaurant.Category);
            restaurantDTO.HasDelivery.Should().Be(restaurant.HasDelivery);
            restaurantDTO.City.Should().Be(restaurant.Address.City);
            restaurantDTO.Street.Should().Be(restaurant.Address.Street);
            restaurantDTO.PostalCode.Should().Be(restaurant.Address.PostalCode);

        }

        [Fact()]
        public void CreateMap_ForCreateRestaurantCommandToRestaurant_MapsCorrectly()
        {

            var command = new CreateRestaurantCommand()
            {

                Name = "Test restaurant",
                Description = "Test Description",
                Category = "Test Category",
                HasDelivery = true,
                ContactEmail = "test@example.com",
                ContactNumber = "123456789",
                City = "Test City",
                Street = "Test Street",
                PostalCode = "12-345"
            };

            //act

            var restaurant = _mapper.Map<Restaurant>(command);


            //assert

            restaurant.Should().NotBeNull();

            restaurant.Name.Should().Be(command.Name);
            restaurant.Description.Should().Be(command.Description);
            restaurant.Category.Should().Be(command.Category);
            restaurant.HasDelivery.Should().Be(command.HasDelivery);
            restaurant.ContactNumber.Should().Be(command.ContactNumber);
            restaurant.ContactEmail.Should().Be(command.ContactEmail);

            restaurant.Address.Should().NotBeNull();
            restaurant.Address.City.Should().Be(restaurant.Address.City);
            restaurant.Address.Street.Should().Be(command.Street);
            restaurant.Address.PostalCode.Should().Be(command.PostalCode);

        }

        [Fact()]
        public void CreateMap_ForUpdateRestaurantCommandToRestaurant_MapsCorrectly()
        {

            var command = new UpdateRestaurantCommand()
            {
                Id = 1,
                Name = "Test restaurant",
                Description = "Test Description",
                HasDelivery = true,
               
            };

            //act

            var restaurant = _mapper.Map<Restaurant>(command);


            //assert

            restaurant.Should().NotBeNull();
            restaurant.Id.Should().Be(command.Id);
            restaurant.Name.Should().Be(command.Name);
            restaurant.Description.Should().Be(command.Description);
            restaurant.HasDelivery.Should().Be(command.HasDelivery);
           
        }
    }
}