using FoodDelivery.Api.Contracts.Restaurants;

namespace FoodDelivery.Api.Services;

public interface IRestaurantSearchService
{
    Task<ServiceResult<PagedResultDto<RestaurantSummaryDto>>> SearchAsync(string? filter, int page, int pageSize,
        string? sortBy, bool descending, CancellationToken cancellationToken);
}
