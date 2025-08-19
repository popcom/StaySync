using StaySync.Domain.Common;
using StaySync.Domain.ValueObjects;

namespace StaySync.Domain.Entities;

public sealed class TravelGroup
{
    public Guid Id { get; private set; }
    public Guid HotelId { get; private set; }
    public GroupId GroupId { get; private set; }
    public DateOnly ArrivalDate { get; private set; }
    public int TravellerCount { get; private set; }

    private TravelGroup() { }

    public TravelGroup(Guid hotelId, GroupId groupId, DateOnly arrivalDate, int travellerCount)
    {
        Id = Guid.NewGuid();
        HotelId = hotelId != default ? hotelId : throw new ArgumentException("HotelId is required.", nameof(hotelId));
        GroupId = groupId;
        ArrivalDate = arrivalDate;
        TravellerCount = Guard.Positive(travellerCount, nameof(travellerCount));
    }

    public void UpdateTravellerCount(int travellerCount) =>
        TravellerCount = Guard.Positive(travellerCount, nameof(travellerCount));
}
