using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Api.Filters;

/// <summary>
/// Extracts and validates the authenticated user's ID from JWT claims.
/// Stores it in HttpContext.Items["UserId"] for controller access.
/// Throws UnauthorizedAccessException if user ID is invalid (caught by GlobalExceptionMiddleware).
/// Usage: [ExtractUserId] or [ServiceFilter(typeof(ExtractUserIdFilter))]
/// </summary>
public class ExtractUserIdFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Get userId from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user ID");
        }

        // Store in HttpContext.Items so controllers can access it
        context.HttpContext.Items["UserId"] = userId;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Can modify result after action execution if needed
    }
}

/// <summary>
/// Attribute for easy filter application
/// Usage: [ExtractUserId]
/// </summary>
public class ExtractUserIdAttribute : ServiceFilterAttribute
{
    public ExtractUserIdAttribute() : base(typeof(ExtractUserIdFilter))
    {
    }
}
