using FoodDelivery.Api.Entities;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Data.Candidatures;

public interface ICandidatureRepository
{
    Task<CandidatureCreationResult> AddAsync(int restaurantId, int chefId, decimal salary,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Candidature>> GetByRestaurantIdAsync(int restaurantId,
        CancellationToken cancellationToken);
    Task<Candidature?> GetByIdAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken);
    Task<CandidatureDecisionStatus> RefuseAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken);
}
