using StaySync.Domain.Common;

namespace StaySync.Domain.Entities;

public sealed class Hotel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Timezone { get; private set; } = default!; // e.g., "Europe/Berlin"
    public string ApiKeyHash { get; private set; } = default!;

    private Hotel() { } // for ORMs

    public Hotel(Guid id, string name, string timezone, string apiKeyHash)
    {
        Id = id == default ? Guid.NewGuid() : id;
        Name = Guard.NotNullOrWhiteSpace(name, nameof(name));
        Timezone = Guard.NotNullOrWhiteSpace(timezone, nameof(timezone));
        ApiKeyHash = Guard.NotNullOrWhiteSpace(apiKeyHash, nameof(apiKeyHash));
    }
}
