using StaySync.Domain.Exceptions;

namespace StaySync.Domain.ValueObjects;

/// <summary>
/// Exactly 6 alphanumeric chars; must not start with '0'; at most 2 letters.
/// Stored uppercase.
/// </summary>
public readonly record struct GroupId
{
    public string Value { get; }

    public GroupId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 6)
            throw new DomainException("GroupId must be exactly 6 characters.");
        if (value[0] == '0') throw new DomainException("GroupId must not start with 0.");
        if (!value.All(char.IsLetterOrDigit))
            throw new DomainException("GroupId must be alphanumeric.");
        if (value.Count(char.IsLetter) > 2)
            throw new DomainException("GroupId may contain at most 2 letters.");

        Value = value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}
