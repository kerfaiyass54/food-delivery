using FoodDelivery.Api.Contracts.Restaurants;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Services;

public sealed class RestaurantSearchService : IRestaurantSearchService
{
    private static readonly string[] SortFields = { "name", "address", "nationality", "budget", "dateOfAdd" };
    private readonly IRestaurantRepository _repository;
    public RestaurantSearchService(IRestaurantRepository repository) => _repository = repository;

    public async Task<ServiceResult<PagedResultDto<RestaurantSummaryDto>>> SearchAsync(string? filter, int page,
        int pageSize, string? sortBy, bool descending, CancellationToken cancellationToken)
    {
        sortBy = string.IsNullOrWhiteSpace(sortBy) ? "name" : sortBy.Trim();
        if (page < 1 || pageSize is < 1 or > 100 || !SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            return ServiceResult<PagedResultDto<RestaurantSummaryDto>>.Invalid("Page must be positive, pageSize must be 1-100, and sortBy must be name, address, nationality, budget, or dateOfAdd.");
        var total = await _repository.CountSearchAsync(filter, cancellationToken);
        var restaurants = await _repository.SearchAsync(filter, page, pageSize, sortBy, descending, cancellationToken);
        return ServiceResult<PagedResultDto<RestaurantSummaryDto>>.Success(new PagedResultDto<RestaurantSummaryDto>(
            restaurants.Select(RestaurantSummaryDto.FromEntity).ToArray(), page, pageSize, total,
            (int)Math.Ceiling(total / (double)pageSize)));
    }
}
