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
                .Include(x => x.Restaurant)
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
                .Include(x => x.Restaurant)
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
            return await SetDecisionAsync(
                restaurantId, candidatureId, CandidatureStatus.Refused, CandidatureDecisionStatus.Refused,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to refuse candidature {CandidatureId} for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            throw;
        }
    }

    public async Task<CandidatureDecisionStatus> AcceptAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await SetDecisionAsync(
                restaurantId, candidatureId, CandidatureStatus.Accepted, CandidatureDecisionStatus.Accepted,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to accept candidature {CandidatureId} for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            throw;
        }
    }

    private async Task<CandidatureDecisionStatus> SetDecisionAsync(int restaurantId, int candidatureId,
        CandidatureStatus newStatus, CandidatureDecisionStatus successfulStatus,
        CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""Candidatures""
                SET ""Status"" = {(int)newStatus}
                WHERE ""Id"" = {candidatureId}
                  AND ""RestaurantId"" = {restaurantId}
                  AND ""Status"" = {(int)CandidatureStatus.Pending}",
            cancellationToken);

        if (affectedRows == 1)
        {
            return successfulStatus;
        }

        var exists = await _dbContext.Candidatures.AnyAsync(
            x => x.Id == candidatureId && x.RestaurantId == restaurantId, cancellationToken);
        return exists ? CandidatureDecisionStatus.AlreadyDecided : CandidatureDecisionStatus.NotFound;
    }

    public async Task<IReadOnlyList<Candidature>> GetByChefIdAsync(int chefId, CancellationToken cancellationToken)
    {
        return await _dbContext.Candidatures.AsNoTracking()
            .Include(x => x.Chef)
            .Include(x => x.Restaurant)
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
