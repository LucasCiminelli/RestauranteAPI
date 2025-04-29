using Restaurants.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Entities
{
    public class Bipper
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public int RestaurantId { get; set; }
        public int ClientId { get; set; }
        public bool IsReady { get; set; }
        public BipperStatus Status { get; set; }
        public BipperType Type { get; set; }
        public bool IsActive { get; set; }


        public Restaurant? Restaurant {  get; set; }
        public Client? Client { get;set; }


    }
}
