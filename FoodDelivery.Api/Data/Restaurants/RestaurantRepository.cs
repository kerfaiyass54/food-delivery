using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Data.Restaurants;

public sealed class RestaurantRepository : IRestaurantRepository
{
    private readonly FoodDeliveryDbContext _dbContext;
    private readonly ILogger<RestaurantRepository> _logger;

    public RestaurantRepository(FoodDeliveryDbContext dbContext, ILogger<RestaurantRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Restaurant?> GetByIdAsync(int restaurantId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Restaurants
                .AsNoTracking()
                .Include(x => x.OperatingDays).ThenInclude(x => x.Day)
                .Include(x => x.OffDays).ThenInclude(x => x.Day)
                .SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<Restaurant?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Restaurants
                .AsNoTracking()
                .Include(x => x.OperatingDays).ThenInclude(x => x.Day)
                .Include(x => x.OffDays).ThenInclude(x => x.Day)
                .SingleOrDefaultAsync(x => x.OwnerId == ownerId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load restaurant managed by owner {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<bool> RestaurantExistsAsync(int restaurantId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Restaurants.AnyAsync(x => x.Id == restaurantId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to check restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<bool> AllDaysExistAsync(IReadOnlyCollection<int> dayIds, CancellationToken cancellationToken)
    {
        if (dayIds.Count == 0)
        {
            return true;
        }

        try
        {
            return await _dbContext.Days.CountAsync(x => dayIds.Contains(x.Id), cancellationToken) == dayIds.Count;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to validate restaurant day identifiers");
            throw;
        }
    }

    public async Task<bool> UpdateCapacityAsync(int restaurantId, int capacity, CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = await _dbContext.Restaurants
                .SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
            if (restaurant is null)
            {
                return false;
            }

            restaurant.Capacity = capacity;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to update capacity for restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<bool> UpdateDaysAsync(int restaurantId, IReadOnlyCollection<int> operatingDayIds,
        IReadOnlyCollection<int> offDayIds, CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = await _dbContext.Restaurants
                .Include(x => x.OperatingDays)
                .Include(x => x.OffDays)
                .SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
            if (restaurant is null)
            {
                return false;
            }

            _dbContext.RestaurantOperatingDays.RemoveRange(restaurant.OperatingDays);
            _dbContext.RestaurantOffDays.RemoveRange(restaurant.OffDays);
            restaurant.OperatingDays = operatingDayIds
                .Select(dayId => new RestaurantOperatingDay { RestaurantId = restaurantId, DayId = dayId })
                .ToList();
            restaurant.OffDays = offDayIds
                .Select(dayId => new RestaurantOffDay { RestaurantId = restaurantId, DayId = dayId })
                .ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to update operating days for restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<bool> UpdateOpeningHoursAsync(int restaurantId, TimeOnly opensAt, TimeOnly closesAt,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = await _dbContext.Restaurants
                .SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
            if (restaurant is null)
            {
                return false;
            }

            restaurant.TimeOpen = opensAt;
            restaurant.TimeClose = closesAt;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to update opening hours for restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<IReadOnlyList<Restaurant>> SearchAsync(string? filter, int page, int pageSize, string sortBy,
        bool descending, CancellationToken cancellationToken)
    {
        var query = FilterRestaurants(filter);
        query = (sortBy.ToLowerInvariant(), descending) switch
        {
            ("address", false) => query.OrderBy(x => x.Address),
            ("address", true) => query.OrderByDescending(x => x.Address),
            ("nationality", false) => query.OrderBy(x => x.Nationality),
            ("nationality", true) => query.OrderByDescending(x => x.Nationality),
            ("budget", false) => query.OrderBy(x => x.Budget),
            ("budget", true) => query.OrderByDescending(x => x.Budget),
            ("dateofadd", false) => query.OrderBy(x => x.DateOfAdd),
            ("dateofadd", true) => query.OrderByDescending(x => x.DateOfAdd),
            (_, false) => query.OrderBy(x => x.Name),
            _ => query.OrderByDescending(x => x.Name)
        };
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountSearchAsync(string? filter, CancellationToken cancellationToken) =>
        FilterRestaurants(filter).CountAsync(cancellationToken);

    private IQueryable<Restaurant> FilterRestaurants(string? filter)
    {
        var query = _dbContext.Restaurants.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var term = filter.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Address.Contains(term) ||
                                     (x.Nationality != null && x.Nationality.Contains(term)));
        }
        return query;
    }
}
