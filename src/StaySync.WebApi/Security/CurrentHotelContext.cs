using StaySync.Application.Interfaces;

namespace StaySync.WebApi.Security;

public sealed class CurrentHotelContext : ICurrentHotelContext
{
    public Guid HotelId { get; internal set; }
}
