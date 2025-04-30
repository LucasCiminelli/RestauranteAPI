using MediatR;
using Restaurants.Application.Bippers.Dtos;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Bippers.Commands.CreateBipper
{
    public class CreateBipperCommand : IRequest<BipperDTO>
    {
        public int RestaurantId { get; set; }
        public int ClientId { get; set; }
        public bool IsReady { get; set; } = false;
        public BipperStatus Status { get; set; } = BipperStatus.Pending;
        public BipperType Type { get; set; }
        public bool isActive { get; set; } = true;

    }
}
