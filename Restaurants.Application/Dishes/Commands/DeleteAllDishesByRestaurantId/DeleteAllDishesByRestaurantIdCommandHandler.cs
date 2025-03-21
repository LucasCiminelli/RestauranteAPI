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

namespace Restaurants.Application.Dishes.Commands.DeleteAllDishesByRestaurantId
{
    public class DeleteAllDishesByRestaurantIdCommandHandler : IRequestHandler<DeleteAllDishesByRestaurantIdCommand>
    {

        private readonly ILogger<DeleteAllDishesByRestaurantIdCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public DeleteAllDishesByRestaurantIdCommandHandler(ILogger<DeleteAllDishesByRestaurantIdCommandHandler> logger, IRestaurantRepository restaurantRepository, IDishRepository dishRepository, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _dishRepository = dishRepository;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task Handle(DeleteAllDishesByRestaurantIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Deleiting all dishes for restaurant {RestaurantId}", request.RestaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
            {

                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            if (!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
            {
                throw new ForbidException();
            }

            await _dishRepository.RemoveAllDishes(restaurant.Dishes!);
        }
    }
}
