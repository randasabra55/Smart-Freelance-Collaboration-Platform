namespace Smart_Freelance_Core.Basics.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointAttribute : Attribute
{
    public string Group { get; }
    public string? Name { get; }
    public EndpointMethod Method { get; }

    public EndpointAttribute(EndpointMethod method, string group, string? name = null)
    {
        Group = group ?? throw new ArgumentNullException(nameof(group));
        Method = method;
        Name = name;
    }
}
