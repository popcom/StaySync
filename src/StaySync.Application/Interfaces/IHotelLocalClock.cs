namespace StaySync.Application.Interfaces;

/// <summary>Returns the hotel-local 'today' date; implementation uses hotel's timezone.</summary>
public interface IHotelLocalClock
{
    Task<DateOnly> TodayAsync(Guid hotelId, CancellationToken ct = default);
}
