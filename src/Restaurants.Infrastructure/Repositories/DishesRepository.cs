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
    public class DishesRepository : IDishRepository
    {
        private readonly RestaurantsDbContext _restaurantsDbContext;

        public DishesRepository(RestaurantsDbContext restaurantsDbContext)
        {
            _restaurantsDbContext = restaurantsDbContext;
        }

        public async Task<int> Create(Dish entity)
        {
            await _restaurantsDbContext.Dishes.AddAsync(entity);
            await _restaurantsDbContext.SaveChangesAsync();
            
            return entity.Id;
        }

        public async Task Delete(Dish entity)
        {
            _restaurantsDbContext.Dishes.Remove(entity);
            await _restaurantsDbContext.SaveChangesAsync();
        }

        public async Task RemoveAllDishes(IEnumerable<Dish> entities)
        {
            _restaurantsDbContext.Dishes.RemoveRange(entities);
            await _restaurantsDbContext.SaveChangesAsync();
        }

        public async Task Update(Dish entity)
        {
            _restaurantsDbContext.Dishes.Update(entity);
            await _restaurantsDbContext.SaveChangesAsync();
        }
    }
}
