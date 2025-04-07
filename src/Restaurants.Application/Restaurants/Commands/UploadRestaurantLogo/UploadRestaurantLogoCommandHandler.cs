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

namespace Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo
{
    public class UploadRestaurantLogoCommandHandler : IRequestHandler<UploadRestaurantLogoCommand>
    {
        private readonly ILogger<UploadRestaurantLogoCommandHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadRestaurantLogoCommandHandler(ILogger<UploadRestaurantLogoCommandHandler> logger, IRestaurantRepository restaurantRepository, IRestaurantAuthorizationService restaurantAuthorizationService, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _restaurantAuthorizationService = restaurantAuthorizationService;
            _blobStorageService = blobStorageService;
        }

        public async Task Handle(UploadRestaurantLogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading Logo for restaurant with id {RestaurantId} with {@UpdatedRestaurant}", request.RestaurantId, request);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant is null)
            {
                _logger.LogWarning($"Restaurant with Id {request.RestaurantId} not found.");
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            if (!_restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
            {
                throw new ForbidException();
            }


            var logoUrl = await _blobStorageService.UploadToBlobAsync(request.File, request.FileName);
            restaurant.LogoUrl = logoUrl;

            await _restaurantRepository.UpdateAsync(restaurant);


        }
    }
}
