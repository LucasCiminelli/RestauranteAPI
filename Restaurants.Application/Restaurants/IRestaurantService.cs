using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDTO>> GetAllRestaurants();
        Task<RestaurantDTO> GetById(int id);
        Task<int> Create(CreateRestaurantDTO restaurant);
    }
}