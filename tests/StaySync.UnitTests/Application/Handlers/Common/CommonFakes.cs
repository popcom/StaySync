using Moq;
using StaySync.Application.Interfaces;

namespace StaySync.UnitTests.Application.Handlers.Common;

internal static class CommonFakes
{
    public static (Guid hotelId, Mock<ICurrentHotelContext> current, Mock<IHotelLocalClock> clock)
        HotelAndClock(DateOnly today)
    {
        var hotelId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var current = new Mock<ICurrentHotelContext>();
        current.SetupGet(x => x.HotelId).Returns(hotelId);

        var clock = new Mock<IHotelLocalClock>();
        clock.Setup(x => x.TodayAsync(hotelId, It.IsAny<CancellationToken>()))
             .ReturnsAsync(today);

        return (hotelId, current, clock);
    }
}
