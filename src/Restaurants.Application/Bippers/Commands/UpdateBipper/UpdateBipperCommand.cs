using MediatR;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.UpdateBipper
{
    public class UpdateBipperCommand : IRequest<BipperDTO>
    {
        public Guid Id { get; set; }
        public int RestaurantId { get; set; }
        public bool? IsReady { get; set; }
        public BipperStatus? Status { get; set; }
        public BipperType? Type { get; set; }
        public bool IsActive { get; set; }

    }
}
