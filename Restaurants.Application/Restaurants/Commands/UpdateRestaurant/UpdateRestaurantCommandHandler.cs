using AutoMapper;
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

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant
{
    public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand>
    {

        private readonly ILogger<UpdateRestaurantCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

        public UpdateRestaurantCommandHandler(ILogger<UpdateRestaurantCommandHandler> logger, IRestaurantRepository restaurantRepository, IMapper mapper, IRestaurantAuthorizationService restaurantAuthorizationService)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _restaurantAuthorizationService = restaurantAuthorizationService;
        }

        public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("updating restaurant with id {RestaurantId} with {@UpdatedRestaurant}", request.Id, request);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id);

            if (restaurant is null)
            {
                _logger.LogWarning($"Restaurant with Id {request.Id} not found.");
                throw new NotFoundException(nameof(Restaurant), request.Id.ToString());
            }

            if (!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
            {
                throw new ForbidException();
            }

            _mapper.Map(request, restaurant);

            await _restaurantRepository.UpdateAsync(restaurant);

        }
    }
}
