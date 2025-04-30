using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.CreateBipper
{
    public class CreateBipperCommandHandler : IRequestHandler<CreateBipperCommand, Bipper>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly ILogger<CreateBipperCommandHandler> _logger;
        private readonly IUserContext _userContext;

        public CreateBipperCommandHandler(IBipperRepository bipperRepository, ILogger<CreateBipperCommandHandler> logger, IUserContext userContext)
        {
            _bipperRepository = bipperRepository;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<Bipper> Handle(CreateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("El usuario debe estar autenticado");
            }


            var bipper = new Bipper
            {
                RestaurantId = request.RestaurantId,
                ClientId = request.ClientId,
                Type = request.Type,
                IsReady = request.IsReady,
                Status = request.Status,
                IsActive = request.isActive
            };

            await _bipperRepository.CreateAsync(bipper);


            return bipper;
        }
    }
}
