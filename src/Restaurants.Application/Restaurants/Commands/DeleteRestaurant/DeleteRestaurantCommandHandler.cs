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

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurant
{
    public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand>
    {

        private readonly ILogger<DeleteRestaurantCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public DeleteRestaurantCommandHandler(ILogger<DeleteRestaurantCommandHandler> logger, IRestaurantRepository restaurantRepository, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting restaurant {RestaurantId}", request.Id);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id);

            if (restaurant is null)
            {
                throw new NotFoundException(nameof(Restaurant), request.Id.ToString());
            }

            if(!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Delete))
            {
                throw new ForbidException();
            }

            await _restaurantRepository.DeleteAsync(restaurant);

        }
    }
}
