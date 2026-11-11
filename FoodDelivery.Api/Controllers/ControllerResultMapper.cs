using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

internal static class ControllerResultMapper
{
    public static ActionResult<T> Map<T>(ControllerBase controller, ServiceResult<T> result,
        ILogger logger) where T : class
    {
        if (result.NotFound)
        {
            return controller.NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        if (result.Conflict)
        {
            return controller.Conflict(new ProblemDetails
            {
                Title = "Request conflicts with the current state",
                Detail = result.Error,
                Status = StatusCodes.Status409Conflict
            });
        }

        if (!result.Succeeded)
        {
            logger.LogWarning("Request rejected: {Error}", result.Error);
            return controller.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return controller.Ok(result.Value);
    }
}
