/*using Smart_Freelance_Core.Basics.Attributes;
using System.Reflection;

namespace Smart_Freelance_API.Infrastructure;
public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GetType().Name;

        return app.MapGroup($"/api/{groupName}")
                        .DisableAntiforgery()
                  .WithGroupName(groupName)
                  .WithTags(groupName);
    }
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointhandlerDelegateFactory = new EndpointHandlerDelegateFactory();
        var endpointGroupType = typeof(EndpointGroupBase);
        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
                                         .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .SingleOrDefault(assembly => assembly.GetName().Name == "Smart_Freelance_API");

        if (applicationAssembly != null)
        {
            var endpointMethods = GetTypesWithEndpointAttribute(applicationAssembly);

            var groupedEndpoints = endpointMethods.GroupBy(type =>
                type.GetCustomAttribute<EndpointAttribute>()?.Group ?? "Default");

            foreach (var group in groupedEndpoints)
            {
                var groupName = group.Key;
                var routeGroup = app.MapGroup($"/api/{groupName}")
                                    .DisableAntiforgery()
                                   .WithGroupName(groupName)
                                   .WithTags(groupName);

                foreach (var requestType in group)
                {
                    var attribute = requestType.GetCustomAttribute<EndpointAttribute>();
                    var responseType = GetResponseType(requestType);
                    var route = attribute?.Name ?? requestType.Name;

                    var filterAttributes = requestType.GetCustomAttributes<EndpointFilterAttribute>()
                                                    .OrderBy(f => f.Order)
                                                    .ToArray();

                    var bindingAttributes = BindingFlags.NonPublic | BindingFlags.Static;
                    var methodArgTypes = new Type[] { typeof(RouteGroupBuilder), typeof(string), typeof(EndpointMethod), typeof(EndpointHandlerDelegateFactory), typeof(EndpointFilterAttribute[]) };

                    MethodInfo? mappingMethod;
                    if (responseType == null)
                    {
                        mappingMethod = typeof(WebApplicationExtensions)
                            .GetMethod(nameof(MapRequestToGroup), bindingAttributes, methodArgTypes)
                            ?.MakeGenericMethod(requestType);
                    }
                    else
                    {
                        mappingMethod = typeof(WebApplicationExtensions)
                            .GetMethod(nameof(MapRequestResponseToGroup), bindingAttributes, methodArgTypes)
                            ?.MakeGenericMethod(requestType, responseType);
                    }

                    if (mappingMethod != null)
                    {
                        mappingMethod.Invoke(null, [routeGroup, route, attribute!.Method, endpointhandlerDelegateFactory, filterAttributes]);
                    }
                }
            }
        }

        return app;
    }

    private static void MapRequestResponseToGroup<TRequest, TResponse>(RouteGroupBuilder group, string route, EndpointMethod method, EndpointHandlerDelegateFactory handlerFactory, EndpointFilterAttribute[] filters)
        where TRequest : IRequest<TResponse>
    {
        var handler = handlerFactory.GetEndpointHandlerDelegate<TRequest, TResponse>(method);
        var routeHandler = MapToGroup(group, method, route, handler);
        ApplyFilters(routeHandler, filters);

    }

    private static void MapRequestToGroup<TRequest>(RouteGroupBuilder group, string route, EndpointMethod method, EndpointHandlerDelegateFactory handlerFactory, EndpointFilterAttribute[] filters)
        where TRequest : IRequest
    {
        var handler = handlerFactory.GetEndpointHandlerDelegate<TRequest>(method);
        var routeHandler = MapToGroup(group, method, route, handler);
        ApplyFilters(routeHandler, filters);

    }

    private static RouteHandlerBuilder MapToGroup(RouteGroupBuilder group, EndpointMethod httpMethod, string route, Delegate handler)
    {
        return httpMethod switch
        {
            EndpointMethod.Get => group.MapGet(route, handler),
            EndpointMethod.Post => group.MapPost(route, handler),
            EndpointMethod.Put => group.MapPut(route, handler),
            EndpointMethod.Delete => group.MapDelete(route, handler),
            EndpointMethod.Patch => group.MapPatch(route, handler),
            _ => throw new NotSupportedException($"Unsupported HTTP Method: {httpMethod}")
        };
    }

    private static Type? GetResponseType(Type commandType)
    {
        var requestInterface = commandType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
        return requestInterface?.GetGenericArguments()[0];
    }

    private static IEnumerable<Type> GetTypesWithEndpointAttribute(Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(t => t.GetCustomAttributes(typeof(EndpointAttribute), false).Length > 0);
    }
    private static void ApplyFilters(RouteHandlerBuilder routeHandler, EndpointFilterAttribute[] filters)
    {
        foreach (var filter in filters)
        {
            filter.ApplyToEndpoint(routeHandler);

        }
    }
}
*/