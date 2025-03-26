using Xunit;
using Restaurants.Application.Dishes.Queries.GetAllDishesForRestaurant;
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

namespace Restaurants.Application.Dishes.Queries.GetAllDishesForRestaurant.Tests
{
    public class GetAllDishesForRestaurantQueryHandlerTests
    {

        private readonly Mock<ILogger<GetAllDishesForRestaurantQueryHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private readonly GetAllDishesForRestaurantQueryHandler _handler;

        public GetAllDishesForRestaurantQueryHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GetAllDishesForRestaurantQueryHandler>>();
            _mapperMock = new Mock<IMapper>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _handler = new GetAllDishesForRestaurantQueryHandler
                (
                    _loggerMock.Object,
                    _restaurantRepositoryMock.Object,
                    _mapperMock.Object
                );

        }

        [Fact()]
        public async Task Handle_GetAllDishesForExistingRestaurant_ReturnIEnumerableListOfDishes()
        {

            //arrange

            var restaurantId = 1;
            var dishes = new List<Dish>()
            {
                new Dish { Id = 1, Name = "Pasta" },
                new Dish { Id = 2, Name = "Pizza" }
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = dishes
            };

            var dishDTOs = new List<DishDTO>
            {
                 new DishDTO { Id = 1, Name = "Pasta" },
                 new DishDTO { Id = 2, Name = "Pizza" }
            };

            var query = new GetAllDishesForRestaurantQuery(restaurantId);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var dishesList = _mapperMock.Setup(m => m.Map<IEnumerable<DishDTO>>(restaurant.Dishes)).Returns(dishDTOs); ;


            //act

            var result = await _handler.Handle(query, CancellationToken.None);


            //assert

            result.Should().NotBeNull();
            result.Should().HaveCount(dishes.Count());
            result.Should().BeAssignableTo<IEnumerable<DishDTO>>();

            _restaurantRepositoryMock.Verify(c => c.GetByIdAsync(restaurantId), Times.Once);
            _mapperMock.Verify(c => c.Map<IEnumerable<DishDTO>>(restaurant.Dishes), Times.Once);
        }


        [Fact()]
        public async Task Handle_GetAllDishesForNonExistingRestaurant_ReturnNotFoundException()
        {

            //arrange

            var restaurantId = 1;
            var dishes = new List<Dish>()
            {
                new Dish { Id = 1, Name = "Pasta" },
                new Dish { Id = 2, Name = "Pizza" }
            };

            var restaurant = new Restaurant()
            {
                Id = restaurantId,
                Dishes = dishes
            };

            var query = new GetAllDishesForRestaurantQuery(restaurantId);

            _restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);


            //act

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);


            //assert

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}