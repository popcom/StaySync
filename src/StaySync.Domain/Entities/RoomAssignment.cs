namespace StaySync.Domain.Entities;

/// <summary>Assignment of a traveller to a room on a specific date.</summary>
public sealed class RoomAssignment
{
    public Guid Id { get; private set; }
    public Guid HotelId { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid TravellerId { get; private set; }
    public DateOnly AssignedOnDate { get; private set; }

    private RoomAssignment() { }

    public RoomAssignment(Guid hotelId, Guid roomId, Guid travellerId, DateOnly assignedOnDate)
    {
        Id = Guid.NewGuid();
        HotelId = hotelId != default ? hotelId : throw new ArgumentException("HotelId is required.", nameof(hotelId));
        RoomId = roomId != default ? roomId : throw new ArgumentException("RoomId is required.", nameof(roomId));
        TravellerId = travellerId != default ? travellerId : throw new ArgumentException("TravellerId is required.", nameof(travellerId));
        AssignedOnDate = assignedOnDate;
    }

    public void ReassignToRoom(Guid targetRoomId)
    {
        if (targetRoomId == default) throw new ArgumentException("Target RoomId is required.", nameof(targetRoomId));
        RoomId = targetRoomId;
    }
}
