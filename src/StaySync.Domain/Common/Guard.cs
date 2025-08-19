namespace StaySync.Domain.Common;

public static class Guard
{
    public static string NotNullOrWhiteSpace(string? value, string paramName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{paramName} is required.", paramName)
            : value;

    public static int Positive(int value, string paramName) =>
        value <= 0
            ? throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be > 0.")
            : value;
}
