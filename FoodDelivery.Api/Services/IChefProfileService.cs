using FoodDelivery.Api.Contracts.Chefs;

namespace FoodDelivery.Api.Services;

public interface IChefProfileService
{
    Task<ServiceResult<ChefDetailsDto>> UpdateAsync(int chefId, UpdateChefProfileRequest request,
        CancellationToken cancellationToken);
}
