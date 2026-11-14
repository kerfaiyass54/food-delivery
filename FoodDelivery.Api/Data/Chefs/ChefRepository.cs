using FoodDelivery.Api.Data.Restaurants;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Data.Chefs;

public sealed class ChefRepository : IChefRepository
{
    private readonly FoodDeliveryDbContext _dbContext;
    private readonly ILogger<ChefRepository> _logger;

    public ChefRepository(FoodDeliveryDbContext dbContext, ILogger<ChefRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Chef>> GetByRestaurantIdAsync(int restaurantId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Chefs.AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load chefs for restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<Chef?> GetByIdAsync(int restaurantId, int chefId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Chefs.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == chefId && x.RestaurantId == restaurantId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load chef {ChefId} for restaurant {RestaurantId}", chefId, restaurantId);
            throw;
        }
    }

    public async Task<Chef?> GetByIdAsync(int chefId, CancellationToken cancellationToken)
    {
        return await _dbContext.Chefs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == chefId, cancellationToken);
    }

    public async Task<bool> UpdateProfileAsync(int chefId, int experience, string speciality, decimal salary,
        CancellationToken cancellationToken)
    {
        var chef = await _dbContext.Chefs.SingleOrDefaultAsync(x => x.Id == chefId, cancellationToken);
        if (chef is null) return false;
        chef.Experience = experience;
        chef.Speciality = speciality;
        chef.Salary = salary;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ChefAssignmentStatus> AssignToRestaurantAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await _dbContext.Restaurants.AnyAsync(x => x.Id == restaurantId, cancellationToken))
            {
                return ChefAssignmentStatus.RestaurantNotFound;
            }

            var chef = await _dbContext.Chefs.SingleOrDefaultAsync(x => x.Id == chefId, cancellationToken);
            if (chef is null)
            {
                return ChefAssignmentStatus.ChefNotFound;
            }

            if (chef.RestaurantId == restaurantId)
            {
                return ChefAssignmentStatus.AlreadyAssigned;
            }

            if (chef.RestaurantId.HasValue)
            {
                return ChefAssignmentStatus.AssignedToAnotherRestaurant;
            }

            chef.RestaurantId = restaurantId;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ChefAssignmentStatus.Assigned;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to assign chef {ChefId} to restaurant {RestaurantId}",
                chefId, restaurantId);
            throw;
        }
    }
}
