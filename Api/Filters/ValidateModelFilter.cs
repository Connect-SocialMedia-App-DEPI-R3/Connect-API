using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

/// <summary>
/// Validates model state automatically before action executes
/// Returns 400 BadRequest with validation errors if model is invalid
/// Usage: Apply globally or use [ServiceFilter(typeof(ValidateModelFilter))]
/// </summary>
public class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new
            {
                message = "Validation failed",
                errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Can log successful actions here
    }
}
