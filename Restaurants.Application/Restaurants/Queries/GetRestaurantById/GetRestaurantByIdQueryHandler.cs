using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
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

        public GetRestaurantByIdQueryHandler(IMapper mapper, ILogger<GetRestaurantByIdQueryHandler> logger, IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<RestaurantDTO> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting restaurant {RestaurantId}", request.Id);
            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException(nameof(Restaurant), request.Id.ToString());

            var restaurantDto = _mapper.Map<RestaurantDTO>(restaurant);

            return restaurantDto;
        }
    }
}
