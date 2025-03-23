using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Respositories;
using Restaurants.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection)
        {

            var searchPhraseLower = searchPhrase?.ToLower();

            var baseQuery = _dbContext.Restaurants
                .Where(r => searchPhraseLower == null || (r.Name!.ToLower().Contains(searchPhraseLower) || r.Description!.ToLower().Contains(searchPhraseLower)));
                

            var totalCount = await baseQuery.CountAsync();

            if (sortBy != null)
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name! },
                    { nameof(Restaurant.Description), r => r.Description! },
                    { nameof(Restaurant.Category), r => r.Category! },
                };

                var selectedColumn = columnsSelector[sortBy];

                baseQuery = sortDirection == SortDirection.Ascending
                            ? baseQuery.OrderBy(selectedColumn)
                            : baseQuery.OrderByDescending(selectedColumn);


            }

            baseQuery.Include(r => r.Dishes);

            var restaurants = await baseQuery
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (restaurants, totalCount);
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
