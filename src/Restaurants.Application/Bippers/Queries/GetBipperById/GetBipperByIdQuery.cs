using MediatR;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Queries.GetBipperById
{
    public class GetBipperByIdQuery : IRequest<Bipper>
    {

        public Guid Id { get; set; }

    }
}
