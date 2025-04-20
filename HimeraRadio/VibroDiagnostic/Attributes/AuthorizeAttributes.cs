using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using VibroDiagnostic.Core.Entities;

namespace VibroDiagnostic.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {

        var auth = context.HttpContext.Request.Headers.Authorization;
        var authEntity = auth.FirstOrDefault();
        if (authEntity.IsNullOrEmpty() || authEntity!.Contains("null"))
        {
            context.HttpContext.Items["User"] = null;
        }
        var user = (User?)context.HttpContext.Items["User"];
        if (user == null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}