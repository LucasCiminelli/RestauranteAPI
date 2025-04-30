using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Queries.GetBipperById
{
    public class GetBipperByIdQueryHandler : IRequestHandler<GetBipperByIdQuery, BipperDTO>
    {

        private readonly IBipperRepository _bipperRepository;
        private readonly ILogger<GetBipperByIdQueryHandler> _logger;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public GetBipperByIdQueryHandler(IBipperRepository bipperRepository, ILogger<GetBipperByIdQueryHandler> logger, IUserContext userContext, IMapper mapper)
        {
            _bipperRepository = bipperRepository;
            _logger = logger;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<BipperDTO> Handle(GetBipperByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User Unauthenticated");
            }

            var bipper = await _bipperRepository.FindByIdAsync(request.Id);

            if (bipper == null) 
            {
                throw new NotFoundException(nameof(Bipper), request.Id.ToString());

            }

            var bipperDTO = _mapper.Map<BipperDTO>(bipper);


            return bipperDTO;
        }
    }
}
