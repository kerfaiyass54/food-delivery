using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Data.Chefs;

namespace FoodDelivery.Api.Services;

public sealed class ChefProfileService : IChefProfileService
{
    private readonly IChefRepository _repository;
    public ChefProfileService(IChefRepository repository) => _repository = repository;

    public async Task<ServiceResult<ChefDetailsDto>> UpdateAsync(int chefId, UpdateChefProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Experience < 0 || request.Experience > 80 || string.IsNullOrWhiteSpace(request.Speciality) ||
            request.Speciality.Length > 100 || request.Salary < 0)
            return ServiceResult<ChefDetailsDto>.Invalid("Experience, speciality, or salary is invalid.");
        if (!await _repository.UpdateProfileAsync(chefId, request.Experience, request.Speciality.Trim(),
                request.Salary, cancellationToken))
            return ServiceResult<ChefDetailsDto>.Missing();
        var chef = await _repository.GetByIdAsync(chefId, cancellationToken);
        return chef is null ? ServiceResult<ChefDetailsDto>.Missing() :
            ServiceResult<ChefDetailsDto>.Success(ChefDetailsDto.FromEntity(chef));
    }
}
