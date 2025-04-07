using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurantById
{
    public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDTO>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<GetRestaurantByIdQueryHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IBlobStorageService _blobStorageService;

        public GetRestaurantByIdQueryHandler(IMapper mapper, ILogger<GetRestaurantByIdQueryHandler> logger, IRestaurantRepository restaurantRepository, IBlobStorageService blobStorageService)
        {
            _mapper = mapper;
            _logger = logger;
            _restaurantRepository = restaurantRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<RestaurantDTO> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting restaurant {RestaurantId}", request.Id);
            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException(nameof(Restaurant), request.Id.ToString());

            var restaurantDto = _mapper.Map<RestaurantDTO>(restaurant);

            restaurantDto.LogoSasUrl = _blobStorageService.GetBlobSasUrl(restaurant.LogoUrl);

            return restaurantDto;
        }
    }
}
