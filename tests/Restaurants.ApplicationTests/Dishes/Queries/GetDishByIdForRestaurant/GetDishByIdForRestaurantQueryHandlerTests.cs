using Xunit;
using Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Restaurants.Domain.Respositories;
using Restaurants.Domain.Entities;
using Restaurants.Application.Dishes.Dtos;
using FluentAssertions;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant.Tests
{
    public class GetDishByIdForRestaurantQueryHandlerTests
    {

        private readonly Mock<ILogger<GetDishByIdForRestaurantQueryHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly GetDishByIdForRestaurantQueryHandler _handler;

        public GetDishByIdForRestaurantQueryHandlerTests()
        {

            _loggerMock = new Mock<ILogger<GetDishByIdForRestaurantQueryHandler>>();
            _mapper = new Mock<IMapper>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();

            _handler = new GetDishByIdForRestaurantQueryHandler
                (
                    _loggerMock.Object,
                    _restaurantRepositoryMock.Object,
                    _mapper.Object
                );

        }

        [Fact()]
        public async Task Handle_GetDishByExistingIdValidRequest_ReturnsDishDTO()
        {
            //arrange

            var dish = new Dish()
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var dishDTO = new DishDTO
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var restaurantId = 1;
            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish> { dish }
            };
            var query = new GetDishByIdForRestaurantQuery(restaurantId, dish.Id);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var dishes = restaurant.Dishes;

            var selectedDish = dishes.FirstOrDefault(d => d.Id == dish.Id);

            _mapper.Setup(m => m.Map<DishDTO>(selectedDish)).Returns(dishDTO);


            //act
            
            var result = await _handler.Handle(query, CancellationToken.None);


            //assert

            result.Should().NotBeNull();
            _mapper.Verify(m => m.Map<DishDTO>(It.IsAny<Dish>()), Times.Once);
            result.Should().BeEquivalentTo(dishDTO);

            result.Name.Should().Be(dishDTO.Name);
            result.Description.Should().Be(dishDTO.Description);
            result.Price.Should().Be(dishDTO.Price);
            result.KiloCalories.Should().Be(dishDTO.KiloCalories);
        }

        [Fact()]
        public async Task Handle_GetDishByNonExistingRestaurant_ReturnsNotFoundException()
        {
            //arrange

            var dish = new Dish()
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var dishDTO = new DishDTO
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var restaurantId = 1;
            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish> { dish }
            };
            var query = new GetDishByIdForRestaurantQuery(restaurantId, dish.Id);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act
            
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();

        }


        [Fact()]
        public async Task Handle_GetDishByNonExistingDishes_ReturnsException()
        {
            //arrange

            var dish = new Dish()
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };



            var restaurantId = 1;
            var restaurant = new Restaurant()
            {
                Id = restaurantId,
            };
            var query = new GetDishByIdForRestaurantQuery(restaurantId, dish.Id);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var dishes = restaurant.Dishes;


            //act

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);


            //assert

            dishes.Should().BeNull();
            _mapper.Verify(m => m.Map<DishDTO>(It.IsAny<Dish>()), Times.Never);
            await act.Should().ThrowAsync<Exception>().WithMessage("This restaurant don't have any dishes");

        }

        [Fact()]
        public async Task Handle_GetDishByNonExistingDish_ReturnsNotFoundException()
        {
            //arrange

            var dish = new Dish()
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var dishDTO = new DishDTO
            {
                Id = 10,
                Name = "Test Dish",
                Description = "Test Description",
                Price = 12.50M,
                KiloCalories = 100
            };

            var restaurantId = 1;
            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = new List<Dish> { dish }
            };
            var query = new GetDishByIdForRestaurantQuery(restaurantId, 100);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var dishes = restaurant.Dishes;

            var selectedDish = dishes.FirstOrDefault(d => d.Id == 100);


            //act

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);


            //assert

            selectedDish.Should().BeNull();
            await act.Should().ThrowAsync<NotFoundException>();


        }
    }
}