using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Touresta.API.Data;

namespace Touresta.API.Filters
{
    /// <summary>
    /// Action filter that blocks ineligible helpers from operational endpoints (booking, chat, etc.).
    /// Checks all 5 eligibility conditions. If drug test is expired, immediately deactivates the helper
    /// (Drug Test Enforcement Point 3 — runtime gate).
    ///
    /// Usage: Apply [TypeFilter(typeof(HelperEligibilityFilter))] to operational endpoints.
    /// </summary>
    public class HelperEligibilityFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            // Only enforce for user-type tokens (admins bypass this check)
            var typeClaim = context.HttpContext.User.FindFirst("type")?.Value;
            if (typeClaim != "user")
            {
                await next();
                return;
            }

            var idClaim = context.HttpContext.User.FindFirst("id")?.Value;
            if (!int.TryParse(idClaim, out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid token." });
                return;
            }

            var helper = await db.Helpers
                .Include(h => h.User)
                .Include(h => h.Languages)
                .Include(h => h.DrugTests)
                .FirstOrDefaultAsync(h => h.UserId == userId);

            if (helper == null)
            {
                context.Result = new ObjectResult(new { message = "No helper profile found." }) { StatusCode = 403 };
                return;
            }

            // Condition 1: User verified
            if (!helper.User.IsVerified)
            {
                context.Result = new ObjectResult(new { message = "User email is not verified." }) { StatusCode = 403 };
                return;
            }

            // Condition 2: Admin approved
            if (!helper.IsApproved)
            {
                context.Result = new ObjectResult(new { message = "Helper is not approved by admin." }) { StatusCode = 403 };
                return;
            }

            // Condition 3 & 4: Active + valid drug test
            var currentDrugTest = helper.DrugTests?.FirstOrDefault(dt => dt.IsCurrent);
            if (currentDrugTest == null || currentDrugTest.ExpiryDate < DateTime.UtcNow)
            {
                // Enforcement Point 3: runtime deactivation
                if (helper.IsActive)
                {
                    helper.IsActive = false;
                    await db.SaveChangesAsync();
                }

                context.Result = new ObjectResult(new { message = "Drug test expired or missing. Helper deactivated." }) { StatusCode = 403 };
                return;
            }

            if (!helper.IsActive)
            {
                context.Result = new ObjectResult(new { message = "Helper account is not active." }) { StatusCode = 403 };
                return;
            }

            // Condition 5: At least one verified language
            var hasVerifiedLanguage = helper.Languages?.Any(l => l.IsVerified) ?? false;
            if (!hasVerifiedLanguage)
            {
                context.Result = new ObjectResult(new { message = "No verified language on profile." }) { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}
