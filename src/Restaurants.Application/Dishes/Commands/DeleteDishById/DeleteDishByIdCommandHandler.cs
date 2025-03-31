using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Dishes.Commands.DeleteDishById
{
    public class DeleteDishByIdCommandHandler : IRequestHandler<DeleteDishByIdCommand>
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<DeleteDishByIdCommandHandler> _logger;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public DeleteDishByIdCommandHandler(IRestaurantRepository restaurantRepository, IDishRepository dishRepository, ILogger<DeleteDishByIdCommandHandler> logger, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _restaurantRepository = restaurantRepository;
            _dishRepository = dishRepository;
            _logger = logger;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task Handle(DeleteDishByIdCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Deleting Dish with id: {DishId} for restaurant with id {RestaurantId}", request.DishId, request.RestaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            var dishes = restaurant.Dishes;

            if (dishes is null)
            {
                throw new Exception("This Restaurant don't have any dishes property");
            }

            var dishToDelete = dishes.FirstOrDefault(d => d.Id == request.DishId);

            if (dishToDelete == null)
            {
                throw new NotFoundException(nameof(Dish), request.DishId.ToString());

            }

            if (!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Delete))
            {
                throw new ForbidException();
            }

            await _dishRepository.Delete(dishToDelete);


        }
    }
}
