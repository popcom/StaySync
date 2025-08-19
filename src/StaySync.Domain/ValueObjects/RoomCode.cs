using System.Text.RegularExpressions;
using StaySync.Domain.Exceptions;

namespace StaySync.Domain.ValueObjects;

/// <summary>4-digit numeric room code, e.g. "0234".</summary>
public readonly partial record struct RoomCode
{
    public string Value { get; }

    public RoomCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !RoomCodeRegex().IsMatch(value))
            throw new DomainException("RoomCode must be exactly 4 digits (0–9).");
        Value = value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[0-9]{4}$")]
    private static partial Regex RoomCodeRegex();
}
