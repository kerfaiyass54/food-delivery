using FoodDelivery.Api.Contracts.Candidatures;
using FoodDelivery.Api.Data.Candidatures;
using FoodDelivery.Api.Data.Chefs;

namespace FoodDelivery.Api.Services;

public sealed class ChefCandidatureService : IChefCandidatureService
{
    private readonly ICandidatureRepository _candidatures;
    private readonly IChefRepository _chefs;
    public ChefCandidatureService(ICandidatureRepository candidatures, IChefRepository chefs)
    { _candidatures = candidatures; _chefs = chefs; }

    public async Task<ServiceResult<CandidatureDetailsDto>> CreateAsync(int chefId,
        CreateChefCandidatureRequest request, CancellationToken cancellationToken)
    {
        if (request.Salary < 0) return ServiceResult<CandidatureDetailsDto>.Invalid("Salary cannot be negative.");
        if (await _chefs.GetByIdAsync(chefId, cancellationToken) is null)
            return ServiceResult<CandidatureDetailsDto>.Missing();
        var result = await _candidatures.AddAsync(request.RestaurantId, chefId, request.Salary, cancellationToken);
        if (result.CandidatureId is not int id) return ServiceResult<CandidatureDetailsDto>.Missing();
        var candidature = await _candidatures.GetByIdAsync(request.RestaurantId, id, cancellationToken);
        return candidature is null ? ServiceResult<CandidatureDetailsDto>.Missing() :
            ServiceResult<CandidatureDetailsDto>.Success(CandidatureDetailsDto.FromEntity(candidature));
    }

    public async Task<ServiceResult<IReadOnlyList<CandidatureDetailsDto>>> GetAllAsync(int chefId,
        CancellationToken cancellationToken)
    {
        if (await _chefs.GetByIdAsync(chefId, cancellationToken) is null)
            return ServiceResult<IReadOnlyList<CandidatureDetailsDto>>.Missing();
        var items = await _candidatures.GetByChefIdAsync(chefId, cancellationToken);
        return ServiceResult<IReadOnlyList<CandidatureDetailsDto>>.Success(items.Select(CandidatureDetailsDto.FromEntity).ToArray());
    }

    public async Task<ServiceResult<object>> DeleteAsync(int chefId, int candidatureId,
        CancellationToken cancellationToken) => await _candidatures.DeleteByChefAsync(chefId, candidatureId,
        cancellationToken) ? ServiceResult<object>.Success(new { Deleted = true }) : ServiceResult<object>.Missing();
}
