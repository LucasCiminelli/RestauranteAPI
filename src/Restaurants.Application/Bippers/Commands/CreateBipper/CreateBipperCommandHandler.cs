using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
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
    public class CreateBipperCommandHandler : IRequestHandler<CreateBipperCommand, BipperDTO>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly ILogger<CreateBipperCommandHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public CreateBipperCommandHandler(IBipperRepository bipperRepository, ILogger<CreateBipperCommandHandler> logger, IUserContext userContext, IMapper mapper)
        {
            _bipperRepository = bipperRepository;
            _logger = logger;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<BipperDTO> Handle(CreateBipperCommand request, CancellationToken cancellationToken)
        {

            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User Unauthenticated");
            }


            var bipper = _mapper.Map<Bipper>(request);
            await _bipperRepository.CreateAsync(bipper);


            var mappedBipper = _mapper.Map<BipperDTO>(bipper);


            return mappedBipper;
        }
    }
}
