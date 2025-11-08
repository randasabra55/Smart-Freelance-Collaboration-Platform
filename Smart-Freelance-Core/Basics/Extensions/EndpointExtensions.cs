using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Core.Basics.Attributes;
using System.Reflection;

namespace Smart_Freelance_Core.Basics.Extensions
{
    public static class EndpointExtensions
    {
        public static void MapMediatREndpoints(this IEndpointRouteBuilder endpoints, params Assembly[] assemblies)
        {
            var group = endpoints.MapGroup("/api");

            var commandTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass || type.IsValueType)
                .Where(type => type.GetCustomAttribute<EndpointAttribute>() != null)
                .ToList();

            foreach (var commandType in commandTypes)
            {
                var endpointAttr = commandType.GetCustomAttribute<EndpointAttribute>()!;
                var tag = endpointAttr.Group;
                var name = endpointAttr.Name ?? commandType.Name.ToLower().Replace("command", "");
                var method = endpointAttr.Method;

                var route = $"{tag}/{name}".ToLower();
                RouteHandlerBuilder builder;

                builder = method switch
                {
                    EndpointMethod.Get => group.MapGet(route, handler: null!),
                    EndpointMethod.Post => group.MapPost(route, handler: null!),
                    EndpointMethod.Put => group.MapPut(route, handler: null!),
                    EndpointMethod.Delete => group.MapDelete(route, handler: null!),
                    EndpointMethod.Patch => group.MapPatch(route, handler: null!),
                    _ => throw new InvalidOperationException($"Unsupported method: {method}")
                };

                ApplyFilters(builder, commandType);

                builder.WithTags(tag);

                builder.Add(b =>
                {
                    b.RequestDelegate = async context =>
                    {
                        var mediator = context.RequestServices.GetRequiredService<IMediator>();
                        var command = await context.Request.ReadFromJsonAsync(commandType, context.RequestAborted);  // 👈 قراءة JSON كـ command

                        if (command == null)
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("Invalid request body");
                            return;
                        }

                        var result = await mediator.Send((dynamic)command, context.RequestAborted);

                        if (result.IsSuccess)
                        {
                            context.Response.StatusCode = 200;
                            //await context.Response.WriteAsJsonAsync(result.Value);
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            // await context.Response.WriteAsJsonAsync(result.Error);
                        }
                    };
                });
            }
        }

        private static void ApplyFilters(RouteHandlerBuilder builder, Type commandType)
        {
            var filters = commandType.GetCustomAttributes<EndpointFilterAttribute>();
            foreach (var filter in filters)
            {
                filter.ApplyToEndpoint(builder);
            }
        }

    }
}
