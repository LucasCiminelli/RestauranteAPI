using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Infrastructure.Repositories
{
    public class BippersRepository : IBipperRepository
    {

        private readonly RestaurantsDbContext _restaurantsDbContext;

        public BippersRepository(RestaurantsDbContext restaurantsDbContext)
        {
            _restaurantsDbContext = restaurantsDbContext;
        }

        public async Task<Bipper> CreateAsync(Bipper bipper)
        {
            await _restaurantsDbContext.Bippers.AddAsync(bipper);
            await _restaurantsDbContext.SaveChangesAsync();

            return bipper;
        }

        public async Task<Bipper?> FindByIdAsync(Guid id)
        {
            var bipper = await _restaurantsDbContext.Bippers
                .Include(b => b.Client)
                .Include(b => b.Restaurant)
                .FirstOrDefaultAsync(b => b.Id == id);

            return bipper;
        }

        public async Task UpdateAsync(Bipper bipper)
        {
            _restaurantsDbContext.Bippers.Update(bipper);
            await _restaurantsDbContext.SaveChangesAsync();
        }
    }
}
