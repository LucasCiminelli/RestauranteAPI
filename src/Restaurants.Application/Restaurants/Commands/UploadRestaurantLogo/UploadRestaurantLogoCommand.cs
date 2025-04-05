using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo
{
    public class UploadRestaurantLogoCommand : IRequest
    {
        public int RestaurantId { get; set; }
        public string FileName { get; set; } = default!;
        public Stream File { get; set; } = default!;
    }
}
