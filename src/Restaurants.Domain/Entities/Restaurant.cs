using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool HasDelivery { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactNumber { get; set; }


        public Address? Address { get; set; }
        public List<Dish>? Dishes { get; set; }

        public List<Bipper>? Bippers { get; set; }


        public User Owner { get; set; } = default!;
        public string OwnerId { get; set; } = default!;
        public string? LogoUrl { get; set; }
    }
}
