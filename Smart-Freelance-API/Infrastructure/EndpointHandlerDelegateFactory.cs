/*using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_Core.Basics.Attributes;
using System.Reflection;

namespace Smart_Freelance_API.Infrastructure;

public class EndpointHandlerDelegateFactory : IEndpointHandlerDelegateFactory
{
    public Delegate GetEndpointHandlerDelegate<TRequest>(EndpointMethod httpMethod) where TRequest : IRequest
        => httpMethod switch
        {
            EndpointMethod.Get => GetHandler<TRequest>(),
            EndpointMethod.Delete => DeleteHandler<TRequest>(),
            EndpointMethod.Post => PostHandler<TRequest>(),
            EndpointMethod.Put => PutHandler<TRequest>(),
            EndpointMethod.Patch => PatchHandler<TRequest>(),
            _ => throw new NotSupportedException(),
        };

    public Delegate GetEndpointHandlerDelegate<TRequest, TResponse>(EndpointMethod httpMethod) where TRequest : IRequest<TResponse>
        => httpMethod switch
        {
            EndpointMethod.Get => GetHandler<TRequest, TResponse>(),
            EndpointMethod.Delete => DeleteHandler<TRequest, TResponse>(),
            EndpointMethod.Post => PostHandler<TRequest, TResponse>(),
            EndpointMethod.Put => PutHandler<TRequest, TResponse>(),
            EndpointMethod.Patch => PatchHandler<TRequest, TResponse>(),
            _ => throw new NotSupportedException(),
        };

    public virtual Delegate GetHandler<TRequest>() where TRequest : IRequest =>
        async (ISender sender, [AsParameters] TRequest request) =>
            await sender.Send(request);

    public virtual Delegate GetHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse> =>
        async (IMediator sender, [AsParameters] TRequest request) =>
            await sender.Send(request);

    public virtual Delegate DeleteHandler<TRequest>() where TRequest : IRequest =>
        async (ISender sender, [AsParameters] TRequest request) =>
            await sender.Send(request);

    public virtual Delegate DeleteHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse> =>
        async (ISender sender, [AsParameters] TRequest request) =>
            await sender.Send(request);

    public virtual Delegate PostHandler<TRequest>() where TRequest : IRequest
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }

    public virtual Delegate PostHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }

    public virtual Delegate PutHandler<TRequest>() where TRequest : IRequest
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }

    public virtual Delegate PutHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }

    public virtual Delegate PatchHandler<TRequest>() where TRequest : IRequest
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }

    public virtual Delegate PatchHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>
    {
        if (HasFormFileProperties<TRequest>())
        {
            return async (ISender sender, [AsParameters] TRequest request) =>
                await sender.Send(request);
        }

        return async (ISender sender, [FromBody] TRequest request) =>
            await sender.Send(request);
    }


    private static bool HasFormFileProperties<TRequest>()
    {
        var requestType = typeof(TRequest);
        var properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(IFormFile).IsAssignableFrom(property.PropertyType) ||
                typeof(IFormFileCollection).IsAssignableFrom(property.PropertyType) ||
                (property.PropertyType.IsGenericType &&
                 property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                 typeof(IFormFile).IsAssignableFrom(property.PropertyType.GetGenericArguments()[0])))
            {
                return true;
            }

            if (property.GetCustomAttribute<FromFormAttribute>() != null)
            {
                return true;
            }
        }

        var constructors = requestType.GetConstructors();
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (typeof(IFormFile).IsAssignableFrom(parameter.ParameterType) ||
                    typeof(IFormFileCollection).IsAssignableFrom(parameter.ParameterType) ||
                    parameter.GetCustomAttribute<FromFormAttribute>() != null)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
*/