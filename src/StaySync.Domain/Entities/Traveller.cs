using StaySync.Domain.Common;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Entities;

public sealed class Traveller
{
    public Guid Id { get; private set; }
    public Guid GroupId { get; private set; }             // FK to TravelGroup
    public string Surname { get; private set; } = default!;   // stored uppercase
    public string FirstName { get; private set; } = default!; // stored uppercase
    public DateOnly DateOfBirth { get; private set; }

    private Traveller() { }

    public Traveller(Guid groupId, TravellerKey travellerKey)
    {
        Id = Guid.NewGuid();
        GroupId = groupId != default ? groupId : throw new ArgumentException("GroupId is required.", nameof(groupId));
        Surname = Guard.NotNullOrWhiteSpace(travellerKey.Surname, nameof(travellerKey.Surname)).Trim().ToUpperInvariant();
        FirstName = Guard.NotNullOrWhiteSpace(travellerKey.FirstName, nameof(travellerKey.FirstName)).Trim().ToUpperInvariant();
        DateOfBirth = travellerKey.DateOfBirth;
    }
}
