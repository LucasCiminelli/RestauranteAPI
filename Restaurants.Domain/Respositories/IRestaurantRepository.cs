using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Domain.Respositories
{
    public interface IRestaurantRepository
    {

        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection);
        Task<Restaurant?> GetByIdAsync(int id);
        Task<int> CreateAsync(Restaurant entity);
        Task UpdateAsync(Restaurant entity);
        Task DeleteAsync(Restaurant entity);

    }
}
