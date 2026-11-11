using FoodDelivery.Api.Contracts.Candidatures;
using FoodDelivery.Api.Contracts.Chefs;

namespace FoodDelivery.Api.Services;

public interface ICandidatureService
{
    Task<ServiceResult<CandidatureDetailsDto>> CreateAsync(int restaurantId, int chefId, decimal salary,
        CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CandidatureDetailsDto>>> GetRestaurantCandidaturesAsync(int restaurantId,
        CancellationToken cancellationToken);
    Task<ServiceResult<CandidatureDetailsDto>> GetByIdAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken);
    Task<ServiceResult<ChefDetailsDto>> GetChefAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken);
    Task<ServiceResult<CandidatureDetailsDto>> RefuseAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken);
}
