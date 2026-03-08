using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Touresta.API.Models.Common;

namespace Touresta.API.Filters
{
    public class ModelValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err =>
                        string.IsNullOrEmpty(err.ErrorMessage)
                            ? $"{e.Key}: Invalid value"
                            : $"{e.Key}: {err.ErrorMessage}"))
                    .ToList();

                var response = ApiResponse.Fail("Validation failed", StatusCodes.Status400BadRequest, errors);

                context.Result = new BadRequestObjectResult(response);
                return;
            }

            await next();
        }
    }
}
