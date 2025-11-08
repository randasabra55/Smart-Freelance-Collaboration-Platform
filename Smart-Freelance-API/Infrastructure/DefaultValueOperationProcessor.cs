/*namespace Smart_Freelance_API.Infrastructure;

using Newtonsoft.Json.Linq;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.ComponentModel;
using System.Reflection;

public class DefaultValueOperationProcessor : IOperationProcessor
{

    public bool Process(OperationProcessorContext context)
    {
        if (context.OperationDescription?.Operation?.Parameters == null)
            return true;

        var methodInfo = context.MethodInfo;
        if (methodInfo == null) return true;

        var parameters = methodInfo.GetParameters();
        var requestParameter = parameters.FirstOrDefault(p =>
            p.ParameterType.GetInterfaces().Any(i =>
                i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(IRequest<>) ||
                    i.GetGenericTypeDefinition().Name == "IRequest`1"
                )) ||
            p.ParameterType.GetInterfaces().Any(i => i.Name == "IRequest")
        );

        if (requestParameter?.ParameterType == null) return true;

        var requestType = requestParameter.ParameterType;
        ProcessTypeForDefaults(requestType, context);

        return true;
    }

    private void ProcessTypeForDefaults(Type type, OperationProcessorContext context)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            ProcessProperty(property, type, context);
        }

        var baseType = type.BaseType;
        if (baseType != null && baseType != typeof(object))
        {
            ProcessTypeForDefaults(baseType, context);
        }
    }

    private void ProcessProperty(PropertyInfo property, Type containingType, OperationProcessorContext context)
    {
        var openApiParam = context.OperationDescription.Operation.Parameters
            .FirstOrDefault(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

        if (openApiParam?.Schema == null) return;

        object? defaultValue = null;

        var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValueAttribute?.Value != null)
        {
            defaultValue = defaultValueAttribute.Value;
        }
        else
        {
            defaultValue = GetPropertyDefaultValue(property, containingType);
        }

        if (defaultValue != null)
        {
            openApiParam.Schema.Default = ConvertToJsonValue(defaultValue);
        }
    }

    private object? GetPropertyDefaultValue(PropertyInfo property, Type type)
    {
        var constructorDefault = GetDefaultFromConstructor(property, type);
        if (constructorDefault != null) return constructorDefault;

        var instanceDefault = GetDefaultFromInstance(property, type);
        if (instanceDefault != null) return instanceDefault;


        return null;
    }

    private object? GetDefaultFromConstructor(PropertyInfo property, Type type)
    {
        try
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                var matchingParam = parameters.FirstOrDefault(p =>
                    string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

                if (matchingParam?.HasDefaultValue == true && matchingParam.DefaultValue != null)
                {
                    return matchingParam.DefaultValue;
                }
            }
        }
        catch
        {
        }

        return null;
    }

    private object? GetDefaultFromInstance(PropertyInfo property, Type type)
    {
        try
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var value = property.GetValue(instance);
                    if (IsNonDefaultValue(value, property.PropertyType))
                    {
                        return value;
                    }
                }
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where(c => c.GetParameters().All(p => p.HasDefaultValue))
                .OrderBy(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var args = parameters.Select(p => p.DefaultValue).ToArray();

                    var instance = Activator.CreateInstance(type, args);
                    if (instance != null)
                    {
                        var value = property.GetValue(instance);
                        if (IsNonDefaultValue(value, property.PropertyType))
                        {
                            return value;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
        }

        return null;
    }


    private bool IsNonDefaultValue(object? value, Type propertyType)
    {
        if (value == null) return false;

        if (propertyType.IsValueType)
        {
            var defaultValue = Activator.CreateInstance(propertyType);
            return !value.Equals(defaultValue);
        }

        return true;
    }

    private object? ConvertToJsonValue(object value)
    {
        return value switch
        {
            null => null,
            string str => str,
            bool boolean => boolean,
            byte or sbyte or short or ushort or int or uint => Convert.ToInt32(value),
            long or ulong => Convert.ToInt64(value),
            float or double or decimal => Convert.ToDouble(value),
            Enum enumValue => enumValue.ToString(),
            DateTime dateTime => dateTime.ToString("O"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
            Guid guid => guid.ToString(),
            _ => JToken.FromObject(value)
        };
    }
}
*/