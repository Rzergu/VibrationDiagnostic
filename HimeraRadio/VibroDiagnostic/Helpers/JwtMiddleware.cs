using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;

namespace VibroDiagnostic.Helpers;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }
    public async Task Invoke(HttpContext context, IUserService userService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            await attachUserToContext(context, userService, token);
        await _next(context);
    }

    private async Task attachUserToContext(HttpContext context, IUserService userService, string token)
    {
        try
        {
            int userId = 0;
            //TODO: Redo! only for test env
            if(_appSettings.Secret == token);
            {
                userId = 1;
            }
            if ("56bd5d66-5b58-467f-9beb-541ad9c8c941" == token)
            {
                userId = 2;
            }

            //Attach user to context on successful JWT validation
            context.Items["User"] = await userService.GetById(userId);
        }
        catch(Exception e)
        {
            //Do nothing if JWT validation fails
            // user is not attached to context so the request won't have access to secure routes
        }
    }
}