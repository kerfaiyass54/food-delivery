using FoodDelivery.Api.Contracts.Candidatures;

namespace FoodDelivery.Api.Services;

public interface IChefCandidatureService
{
    Task<ServiceResult<CandidatureDetailsDto>> CreateAsync(int chefId, CreateChefCandidatureRequest request,
        CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CandidatureDetailsDto>>> GetAllAsync(int chefId,
        CancellationToken cancellationToken);
    Task<ServiceResult<object>> DeleteAsync(int chefId, int candidatureId, CancellationToken cancellationToken);
}
