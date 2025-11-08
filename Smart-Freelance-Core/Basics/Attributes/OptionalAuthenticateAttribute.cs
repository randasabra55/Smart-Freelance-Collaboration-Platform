
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Basics.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class OptionalAuthenticateAttribute : EndpointFilterAttribute
{
    public override void ApplyToEndpoint(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
            {
                return await next(context);
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return (Result)ErrorCode.NotAuthorized;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var tokenValidator = httpContext.RequestServices.GetRequiredService<ITokenValidator>();

            var validationResult = await tokenValidator.ValidateAccessTokenAsync(token);
            if (validationResult.IsSuccess)
                return await next(context);

            return (Result)validationResult.Error;
        });
    }
}
