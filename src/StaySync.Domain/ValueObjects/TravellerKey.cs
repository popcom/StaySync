using StaySync.Domain.Common;

namespace StaySync.Domain.ValueObjects;

/// <summary>Traveller identity within a travel group.</summary>
public readonly record struct TravellerKey
{
    public string Surname { get; }
    public string FirstName { get; }
    public DateOnly DateOfBirth { get; }

    public TravellerKey(string surname, string firstName, DateOnly dateOfBirth)
    {
        Surname = Guard.NotNullOrWhiteSpace(surname, nameof(surname)).Trim().ToUpperInvariant();
        FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName)).Trim().ToUpperInvariant();
        DateOfBirth = dateOfBirth;
    }

    public override string ToString() => $"{Surname}/{FirstName}/{DateOfBirth:yyyy-MM-dd}";
}
