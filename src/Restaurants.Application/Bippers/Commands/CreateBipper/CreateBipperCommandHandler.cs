using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Common.Interfaces;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.CreateBipper
{
    public class CreateBipperCommandHandler : IRequestHandler<CreateBipperCommand, BipperDTO>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly ILogger<CreateBipperCommandHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IBipperAuthorizationService _bipperAuthorizationService;
        private readonly IClientRepository _clientRepository;

        public CreateBipperCommandHandler(IBipperRepository bipperRepository, ILogger<CreateBipperCommandHandler> logger, IUserContext userContext, IMapper mapper, IRestaurantRepository restaurantRepository, IBipperAuthorizationService bipperAuthorizationService, IClientRepository clientRepository)
        {
            _bipperRepository = bipperRepository;
            _logger = logger;
            _userContext = userContext;
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
            _bipperAuthorizationService = bipperAuthorizationService;
            _clientRepository = clientRepository;
        }

        public async Task<BipperDTO> Handle(CreateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();

            _bipperAuthorizationService.EnsureIsAuthenticated(currentUser);

            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if(restaurant == null)
            {
                throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
            }

            var client = await _clientRepository.FindByIdAsync(request.ClientId);

            if(client  == null)
            {
                throw new NotFoundException(nameof(Client), request.RestaurantId.ToString());
            }

            var bipper = _mapper.Map<Bipper>(request);
            await _bipperRepository.CreateAsync(bipper);


            var mappedBipper = _mapper.Map<BipperDTO>(bipper);


            return mappedBipper;
        }
    }
}
