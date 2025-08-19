namespace StaySync.Application.Interfaces;

/// <summary>Provides the current HotelId (resolved from API key in WebApi).</summary>
public interface ICurrentHotelContext
{
    Guid HotelId { get; }
}
