using StaySync.Domain.Common;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Entities;

public sealed class Room
{
    public Guid Id { get; private set; }
    public Guid HotelId { get; private set; }
    public RoomCode RoomCode { get; private set; }
    public int BedCount { get; private set; }

    private Room() { }

    public Room(Guid hotelId, RoomCode roomCode, int bedCount)
    {
        Id = Guid.NewGuid();
        HotelId = hotelId != default ? hotelId : throw new ArgumentException("HotelId is required.", nameof(hotelId));
        RoomCode = roomCode;
        BedCount = Guard.Positive(bedCount, nameof(bedCount));
    }

    public void UpdateBedCount(int bedCount) => BedCount = Guard.Positive(bedCount, nameof(bedCount));
}
