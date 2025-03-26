using FluentValidation;
using Restaurants.Application.Restaurants.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Queries.GetAllRestaurants
{
    public class GetAllRestaurantsQueryValidator : AbstractValidator<GetAllRestaurantsQuery>
    {

        private int[] allowedPageSizes = [5, 10, 15, 30];
        private string[] allowedSortByColumns = [nameof(RestaurantDTO.Name), nameof(RestaurantDTO.Description), nameof(RestaurantDTO.Category)];
        public GetAllRestaurantsQueryValidator()
        {

            RuleFor(r => r.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(r => r.PageSize)
                .Must(value => allowedPageSizes.Contains(value))
                .WithMessage($"Page Size must be in [{string.Join(",", allowedPageSizes)}]");

            RuleFor(r => r.SortBy)
                .Must(value => allowedSortByColumns.Contains(value))
                .When(q => q.SortBy != null)
                .WithMessage($"Page Size must be in [{string.Join(",", allowedSortByColumns)}]");
        }
    }
}
