using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RAFIQ.API.Data;

namespace RAFIQ.API.Filters
{
   
    public class HelperEligibilityFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var typeClaim = context.HttpContext.User.FindFirst("type")?.Value;
            if (typeClaim != "user")
            {
                await next();
                return;
            }

            var userId = context.HttpContext.User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
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

            if (!helper.User.IsVerified)
            {
                context.Result = new ObjectResult(new { message = "User email is not verified." }) { StatusCode = 403 };
                return;
            }

            if (!helper.IsApproved)
            {
                context.Result = new ObjectResult(new { message = "Helper is not approved by admin." }) { StatusCode = 403 };
                return;
            }

            if (helper.IsBanned)
            {
                context.Result = new ObjectResult(new
                {
                    message = "Helper account is banned by admin.",
                    reason = helper.BanReason
                })
                { StatusCode = 403 };
                return;
            }

            if (helper.IsSuspended)
            {
                context.Result = new ObjectResult(new
                {
                    message = "Helper account is suspended by admin.",
                    reason = helper.SuspensionReason
                })
                { StatusCode = 403 };
                return;
            }

            var currentDrugTest = helper.DrugTests?.FirstOrDefault(dt => dt.IsCurrent);
            if (currentDrugTest == null || currentDrugTest.ExpiryDate < DateTime.UtcNow)
            {
                if (helper.IsActive)
                {
                    helper.IsActive = false;
                    await db.SaveChangesAsync();
                }

                context.Result = new ObjectResult(new
                {
                    message = "Drug test expired or missing. Helper deactivated."
                })
                { StatusCode = 403 };
                return;
            }

            if (!helper.IsActive)
            {
                context.Result = new ObjectResult(new { message = "Helper account is not active." })
                { StatusCode = 403 };
                return;
            }

            var hasVerifiedLanguage = helper.Languages?.Any(l => l.IsVerified) ?? false;
            if (!hasVerifiedLanguage)
            {
                context.Result = new ObjectResult(new { message = "No verified language on profile." })
                { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}