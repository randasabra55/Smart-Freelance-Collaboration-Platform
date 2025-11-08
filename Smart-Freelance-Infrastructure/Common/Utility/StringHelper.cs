namespace Smart_Freelance_Infrastructure.Common.Helpers;
public static class StringHelper
{
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && !string.IsNullOrEmpty(value);
    }


}
