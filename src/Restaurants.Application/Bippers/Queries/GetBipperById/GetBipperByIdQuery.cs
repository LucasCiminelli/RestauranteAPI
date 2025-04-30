using MediatR;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Queries.GetBipperById
{
    public class GetBipperByIdQuery : IRequest<BipperDTO>
    {

        public Guid Id { get; set; }

    }
}
