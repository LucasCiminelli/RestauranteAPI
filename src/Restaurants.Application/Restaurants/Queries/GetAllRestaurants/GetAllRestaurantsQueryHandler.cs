using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Common;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Queries.GetAllRestaurants
{
    public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, PagedResults<RestaurantDTO>>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRestaurantsQueryHandler> _logger;
        private readonly IRestaurantRepository _restaurantRepository;

        public GetAllRestaurantsQueryHandler(IMapper mapper, ILogger<GetAllRestaurantsQueryHandler> logger, IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<PagedResults<RestaurantDTO>> Handle(GetAllRestaurantsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all Restaurants");

            var (restaurants, totalCount) = await _restaurantRepository.GetAllMatchingAsync(request.SearchPhrase, request.PageSize, request.PageNumber, request.SortBy, request.SortDirection);
            var restaurantsDto = _mapper.Map<IEnumerable<RestaurantDTO>>(restaurants);

            var result = new PagedResults<RestaurantDTO>(restaurantsDto, totalCount, request.PageSize, request.PageNumber);

            return result;
        }
    }
}
