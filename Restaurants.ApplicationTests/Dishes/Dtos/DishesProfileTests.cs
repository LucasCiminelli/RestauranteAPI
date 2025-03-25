using Xunit;
using Restaurants.Application.Dishes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Restaurants.Domain.Entities;
using FluentAssertions;
using Restaurants.Application.Dishes.Commands.CreateDish;

namespace Restaurants.Application.Dishes.Dtos.Tests
{
    public class DishesProfileTests
    {

        private readonly IMapper _mapper;

        public DishesProfileTests()
        {

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DishesProfile>();
            });

            _mapper = mapperConfiguration.CreateMapper();

        }

        [Fact()]
        public void CreateMap_ForDishToDishDTO_MapsCorrectly()
        {

            //arrange

            var dish = new Dish()
            {
                Id = 1,
                Name = "Test Dish",
                Description = "Test description",
                Price = 12.50M,
                KiloCalories = 100
            };


            //act

            var dishDTO = _mapper.Map<DishDTO>(dish);


            //assert

            dishDTO.Should().NotBeNull();
            dishDTO.Id.Should().Be(dish.Id);
            dishDTO.Name.Should().Be(dish.Name);
            dishDTO.Description.Should().Be(dish.Description);
            dishDTO.Price.Should().Be(dish.Price);
            dishDTO.KiloCalories.Should().Be(dish.KiloCalories);

        }

        [Fact()]
        public void CreateMap_ForDishToCreateDishCommand_MapsCorrectly()
        {

            var command = new CreateDishCommand()
            {
                Name = "Test Dish",
                Description = "Test description",
                Price = 12.50M,
                KiloCalories = 100
            };


            //act

            var dishDTO = _mapper.Map<Dish>(command);


            //assert

            dishDTO.Should().NotBeNull();
            dishDTO.Name.Should().Be(command.Name);
            dishDTO.Description.Should().Be(command.Description);
            dishDTO.Price.Should().Be(command.Price);
            dishDTO.KiloCalories.Should().Be(command.KiloCalories);

        }
    }
}