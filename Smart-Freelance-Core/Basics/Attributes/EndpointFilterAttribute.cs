using Microsoft.AspNetCore.Builder;

namespace Smart_Freelance_Core.Basics.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class EndpointFilterAttribute : Attribute
{
    public int Order { get; set; } = 0;
    public abstract void ApplyToEndpoint(RouteHandlerBuilder builder);
}
