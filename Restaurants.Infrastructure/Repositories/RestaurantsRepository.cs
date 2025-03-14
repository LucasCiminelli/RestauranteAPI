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
    public class RestaurantsRepository : IRestaurantRepository
    {

        private readonly RestaurantsDbContext _dbContext;

        public RestaurantsRepository(RestaurantsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateAsync(Restaurant entity)
        {
            await _dbContext.Restaurants.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;

        }

        public async Task DeleteAsync(Restaurant entity)
        {
             _dbContext.Restaurants.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            var restaurants = await _dbContext.Restaurants
                .Include(r => r.Dishes)
                .ToListAsync();

            return restaurants;
        }

        public async Task<Restaurant> GetByIdAsync(int id)
        {
            var restaurant = await _dbContext.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(x => x.Id == id);

            return restaurant!;
        }

        public async Task UpdateAsync(Restaurant entity)
        {
            _dbContext.Restaurants.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
