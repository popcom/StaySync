using StaySync.Application.Interfaces;
using StaySync.Domain.Interfaces.Repositories;
using TimeZoneConverter;

namespace StaySync.WebApi.Time;

public sealed class HotelLocalClock(IHotelRepository hotels) : IHotelLocalClock
{
    public async Task<DateOnly> TodayAsync(Guid hotelId, CancellationToken ct = default)
    {
        var hotel = await hotels.GetByIdAsync(hotelId, ct) ?? throw new InvalidOperationException("Hotel missing.");
        var tz = TZConvert.GetTimeZoneInfo(hotel.Timezone); // e.g., "Europe/Berlin" , handles IANA or Windows ids
        var nowUtc = DateTimeOffset.UtcNow;
        var local = TimeZoneInfo.ConvertTime(nowUtc, tz);
        return DateOnly.FromDateTime(local.Date);
    }
}
