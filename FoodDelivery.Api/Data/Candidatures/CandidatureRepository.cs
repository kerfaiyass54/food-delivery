using FoodDelivery.Api.Data.Restaurants;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Data.Candidatures;

public sealed class CandidatureRepository : ICandidatureRepository
{
    private readonly FoodDeliveryDbContext _dbContext;
    private readonly ILogger<CandidatureRepository> _logger;

    public CandidatureRepository(FoodDeliveryDbContext dbContext, ILogger<CandidatureRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CandidatureCreationResult> AddAsync(int restaurantId, int chefId, decimal salary,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = await _dbContext.Restaurants
                .SingleOrDefaultAsync(x => x.Id == restaurantId, cancellationToken);
            if (restaurant is null)
            {
                return new CandidatureCreationResult(CandidatureCreationStatus.RestaurantNotFound);
            }

            var chef = await _dbContext.Chefs
                .SingleOrDefaultAsync(x => x.Id == chefId, cancellationToken);
            if (chef is null)
            {
                return new CandidatureCreationResult(CandidatureCreationStatus.ChefNotFound);
            }

            var candidature = new Candidature
            {
                RestaurantId = restaurantId,
                Restaurant = restaurant,
                ChefId = chefId,
                Chef = chef,
                Salary = salary,
                Date = DateTimeOffset.UtcNow,
                Status = CandidatureStatus.Pending
            };

            _dbContext.Candidatures.Add(candidature);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CandidatureCreationResult(CandidatureCreationStatus.Created, candidature.Id);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Failed to create candidature for chef {ChefId} at restaurant {RestaurantId}", chefId, restaurantId);
            throw;
        }
    }

    public async Task<IReadOnlyList<Candidature>> GetByRestaurantIdAsync(int restaurantId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Candidatures.AsNoTracking()
                .Include(x => x.Chef)
                .Where(x => x.RestaurantId == restaurantId)
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load candidatures for restaurant {RestaurantId}", restaurantId);
            throw;
        }
    }

    public async Task<Candidature?> GetByIdAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Candidatures.AsNoTracking()
                .Include(x => x.Chef)
                .SingleOrDefaultAsync(x => x.Id == candidatureId && x.RestaurantId == restaurantId,
                    cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load candidature {CandidatureId} for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            throw;
        }
    }

    public async Task<CandidatureDecisionStatus> RefuseAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        try
        {
            var candidature = await _dbContext.Candidatures
                .SingleOrDefaultAsync(x => x.Id == candidatureId && x.RestaurantId == restaurantId,
                    cancellationToken);
            if (candidature is null)
            {
                return CandidatureDecisionStatus.NotFound;
            }

            if (candidature.Status != CandidatureStatus.Pending)
            {
                return CandidatureDecisionStatus.AlreadyDecided;
            }

            candidature.Status = CandidatureStatus.Refused;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return CandidatureDecisionStatus.Refused;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to refuse candidature {CandidatureId} for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            throw;
        }
    }

    public async Task<IReadOnlyList<Candidature>> GetByChefIdAsync(int chefId, CancellationToken cancellationToken)
    {
        return await _dbContext.Candidatures.AsNoTracking().Include(x => x.Chef)
            .Where(x => x.ChefId == chefId).OrderByDescending(x => x.Date).ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteByChefAsync(int chefId, int candidatureId, CancellationToken cancellationToken)
    {
        var candidature = await _dbContext.Candidatures.SingleOrDefaultAsync(
            x => x.ChefId == chefId && x.Id == candidatureId, cancellationToken);
        if (candidature is null) return false;
        _dbContext.Candidatures.Remove(candidature);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
