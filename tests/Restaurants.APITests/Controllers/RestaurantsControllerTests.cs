using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Restaurants.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Moq;
using Restaurants.Domain.Respositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restaurants.Domain.Entities;
using Restaurants.APITests;
using System.Net.Http.Json;
using Restaurants.Application.Restaurants.Dtos;
using Azure;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Users;

namespace Restaurants.API.Controllers.Tests
{
    [TestClass()]
    public class RestaurantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IRestaurantRepository> _restarantRepositoryMock = new();
        private readonly Mock<IUserContext> _userContextMock = new();

        public RestaurantsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {

                    _userContextMock.Setup(c => c.GetCurrentUser()).Returns(new CurrentUser("1", "test@example.com", ["Admin", "Owner"], null, null));

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantRepository), _ => _restarantRepositoryMock.Object));
                    services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext), _ => _userContextMock.Object));
                });
            });
        }

        [Fact()]
        public async Task GetAll_ForValidRequest_Returns200Ok()
        {

            //arrange

            var client = _factory.CreateClient();



            //act

            var result = await client.GetAsync("/api/restaurants?pageNumber=1&pageSize=10");



            //assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);


        }

        [Fact()]
        public async Task GetAll_ForInvalidRequest_Returns400BadRequest()
        {

            //arrange

            var client = _factory.CreateClient();



            //act

            var result = await client.GetAsync("/api/restaurants");



            //assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);


        }

        [Fact()]
        public async Task GetById_ForNonExistingId_Returns404NotFound()
        {
            //arrange

            var id = 1234;


            _restarantRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Restaurant?)null);

            var client = _factory.CreateClient();


            //act

            var result = await client.GetAsync($"/api/restaurants/{id}");


            //assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async Task GetById_ForExistingId_Returns200Ok()
        {
            //arrange

            var id = 1234;

            var restaurant = new Restaurant
            {
                Id = id,
                Name = "Test Restaurant",
                Description = "Test Description"
            };

            _restarantRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(restaurant);

            var client = _factory.CreateClient();


            //act

            var result = await client.GetAsync($"/api/restaurants/{id}");
            var restaurantDTO = await result.Content.ReadFromJsonAsync<RestaurantDTO>();

            //assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            restaurantDTO.Should().NotBeNull();
            restaurantDTO.Name.Should().Be("Test Restaurant");
            restaurantDTO.Description.Should().Be("Test Description");
        }

        [Fact()]
        public async Task Create_ForValidRequest_ReturnStatus201Created()
        {
            // Arrange
            var restaurant = new CreateRestaurantCommand
            {
                Name = "Test",
                Category = "Italian",
                PostalCode = "12-345",
                ContactEmail = "Test@test.com",
                Description = "Test Description"
            };

            _restarantRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Restaurant>())).ReturnsAsync(123);

            var client = _factory.CreateClient();

            // Act
            var result = await client.PostAsJsonAsync($"/api/restaurants/", restaurant);
            var location = result.Headers.Location;

            // Assert
            _restarantRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Restaurant>()), Times.Once());
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            location.Should().NotBeNull();
            location.AbsolutePath.Should().Be("/api/restaurants/123");


        }
    }
}