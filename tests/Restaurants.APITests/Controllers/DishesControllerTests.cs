using Xunit;
using Restaurants.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Restaurants.Domain.Respositories;
using Restaurants.Application.Users;
using Restaurants.Domain.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Restaurants.APITests;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restaurants.Domain.Entities;
using Restaurants.Application.Dishes.Commands.CreateDish;
using Restaurants.Domain.Constants;
using System.Net.Http.Json;
using FluentAssertions;
using Restaurants.Application.Dishes.Dtos;

namespace Restaurants.API.Controllers.Tests
{
    [TestClass()]
    public class DishesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IRestaurantRepository> _restaurantsRepositoryMock = new();
        private readonly Mock<IDishRepository> _dishRepositoryMock = new();
        private readonly Mock<IUserContext> _userContextMock = new();
        private readonly Mock<IRestaurantAuthorizationService> _authorizationServiceMock = new();

        public DishesControllerTests(WebApplicationFactory<Program> factory)
        {

            _factory = factory.WithWebHostBuilder(builder =>
            {

                builder.ConfigureTestServices(services =>
                {

                    _userContextMock.Setup(c => c.GetCurrentUser()).Returns(new CurrentUser("1", "test@example.com", ["Admin", "Owner"], null, null));

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantRepository), _ => _restaurantsRepositoryMock.Object));
                    services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext), _ => _userContextMock.Object));
                    services.Replace(ServiceDescriptor.Scoped(typeof(IDishRepository), _ => _dishRepositoryMock.Object));
                    services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantAuthorizationService), _ => _authorizationServiceMock.Object));

                });

            });

        }

        [Fact()]
        public async Task Create_ForValidRequest_Return201NoContent()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };


            var command = new CreateDishCommand
            {
                Name = "Test",
                Description = "Test",
                Price = 12.50M,
                KiloCalories = 100,
                RestaurantId = restaurantId
            };


            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Create)).Returns(true);
            _dishRepositoryMock.Setup(d => d.Create(It.IsAny<Dish>())).ReturnsAsync(123);


            var client = _factory.CreateClient();



            var result = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/dishes", command);
            var location = result.Headers.Location;


            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            _authorizationServiceMock.Verify(a => a.Authorize(restaurant, ResourceOperation.Create), Times.Once());
            _dishRepositoryMock.Verify(d => d.Create(It.IsAny<Dish>()), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            location.Should().NotBeNull();
            location.AbsolutePath.Should().Be("/api/restaurants/1/dishes/123");



        }

        [Fact()]
        public async Task Create_ForInvalidRequest_Return400BadRequest()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };


            var command = new CreateDishCommand
            {
                Name = "T",
                Description = "T",
                Price = -1,
                KiloCalories = -1,
                RestaurantId = restaurantId
            };


            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Create)).Returns(true);
            _dishRepositoryMock.Setup(d => d.Create(It.IsAny<Dish>())).ReturnsAsync(123);


            var client = _factory.CreateClient();



            var result = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/dishes", command);
            var location = result.Headers.Location;



            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            location.Should().BeNull();



        }

        [Fact()]
        public async Task Create_ForUnauthorizedUser_Return403Forbidden()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId
            };


            var command = new CreateDishCommand
            {
                Name = "Test",
                Description = "Test",
                Price = 12.50M,
                KiloCalories = 100,
                RestaurantId = restaurantId
            };


            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Create)).Returns(false);

            var client = _factory.CreateClient();



            var result = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/dishes", command);
            var location = result.Headers.Location;



            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
            location.Should().BeNull();

        }

        [Fact()]
        public async Task GetAllByRestaurantId_ForExistingRestaurant_Return200OK()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test"
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var client = _factory.CreateClient();



            var result = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes");



            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact()]
        public async Task GetAllByRestaurantId_ForNonExistingRestaurant_Return404NotFound()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test"
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

            var client = _factory.CreateClient();



            var result = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes");



            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);


        }

        [Fact()]
        public async Task GetById_ForExistingRestaurantAndExistingDish_Return200OK()
        {

            //Arrange

            var restaurantId = 1;
            var dishId = 2;

            var dish = new Dish
            {
                Id = dishId,
                Name = "DishTest",
                Description = "DishTest",
                Price = 12.50M,
                KiloCalories = 100,

            };

            var dishesList = new List<Dish> { dish };

            var dishDTO = new DishDTO
            {
                Id = dishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                KiloCalories = dish.KiloCalories
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test",
                Dishes = dishesList
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var client = _factory.CreateClient();


            // Act
            var response = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes/{dishId}");


            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var responseDishDTO = await response.Content.ReadFromJsonAsync<DishDTO>();
            responseDishDTO.Should().BeEquivalentTo(dishDTO);
        }

        [Fact()]
        public async Task GetById_ForNonExistingRestaurant_Return404NotFound()
        {

            //Arrange

            var restaurantId = 1;
            var dishId = 2;

            var dish = new Dish
            {
                Id = dishId,
                Name = "DishTest",
                Description = "DishTest",
                Price = 12.50M,
                KiloCalories = 100,

            };

            var dishesList = new List<Dish> { dish };

            var dishDTO = new DishDTO
            {
                Id = dishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                KiloCalories = dish.KiloCalories
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test",
                Dishes = dishesList
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

            var client = _factory.CreateClient();


            // Act

            var response = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes/{dishId}");


            // Assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact()]
        public async Task GetById_ForNonExistingDishes_Return500InternalServerError()
        {
            //Arrange

            var restaurantId = 1;
            var dishId = 2;

            var dish = new Dish
            {
                Id = dishId,
                Name = "DishTest",
                Description = "DishTest",
                Price = 12.50M,
                KiloCalories = 100,

            };

            var dishDTO = new DishDTO
            {
                Id = dishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                KiloCalories = dish.KiloCalories
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test",
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var client = _factory.CreateClient();


            // Act

            var response = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes/{dishId}");


            // Assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);


        }

        [Fact()]
        public async Task GetById_ForExistingRestaurantAndNonExistingDish_Return404NotFound()
        {

            //Arrange

            var restaurantId = 1;
            var dishId = 2;

            var dish = new Dish
            {
                Id = dishId,
                Name = "DishTest",
                Description = "DishTest",
                Price = 12.50M,
                KiloCalories = 100,

            };

            var dishesList = new List<Dish> { dish };

            var dishDTO = new DishDTO
            {
                Id = dishId,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                KiloCalories = dish.KiloCalories
            };

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test",
                Dishes = dishesList
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var client = _factory.CreateClient();


            // Act
            var response = await client.GetAsync($"/api/restaurants/{restaurantId}/dishes/3");


            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async Task DeleteAllDishes_ForExistingRestaurant_Return204NoContent()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test"
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(true);

            var client = _factory.CreateClient();



            var result = await client.DeleteAsync($"/api/restaurants/{restaurantId}/dishes");



            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            _authorizationServiceMock.Verify(a => a.Authorize(restaurant,ResourceOperation.Delete), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        }

        [Fact()]
        public async Task DeleteAllDishes_ForNonExistingRestaurant_Return404NoContent()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test"
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);

            var client = _factory.CreateClient();

            var result = await client.DeleteAsync($"/api/restaurants/{restaurantId}/dishes");

            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact()]
        public async Task DeleteAllDishes_ForUnauthorizedUser_Return403Forbidden()
        {

            var restaurantId = 1;

            var restaurant = new Restaurant
            {
                Id = restaurantId,
                Name = "test",
                Description = "test"
            };

            _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _authorizationServiceMock.Setup(a => a.Authorize(restaurant, ResourceOperation.Delete)).Returns(false);

            var client = _factory.CreateClient();



            var result = await client.DeleteAsync($"/api/restaurants/{restaurantId}/dishes");



            _restaurantsRepositoryMock.Verify(r => r.GetByIdAsync(restaurantId), Times.Once());
            _authorizationServiceMock.Verify(a => a.Authorize(restaurant, ResourceOperation.Delete), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);



        }
    }
}