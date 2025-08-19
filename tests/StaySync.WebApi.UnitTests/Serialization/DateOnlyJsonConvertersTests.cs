using System.Text.Json;
using FluentAssertions;
using StaySync.WebApi.Serialization;
using Xunit;

namespace StaySync.WebApi.UnitTests.Serialization;

public class DateOnlyJsonConvertersTests
{
    [Fact]
    public void Roundtrip_DateOnly()
    {
        var opts = new JsonSerializerOptions();
        opts.Converters.Add(new DateOnlyJsonConverter());

        var d = new DateOnly(2025, 8, 14);
        var json = JsonSerializer.Serialize(d, opts);
        json.Should().Be("\"2025-08-14\"");

        var back = JsonSerializer.Deserialize<DateOnly>(json, opts);
        back.Should().Be(d);
    }

    [Fact]
    public void Roundtrip_Nullable_DateOnly()
    {
        var opts = new JsonSerializerOptions();
        opts.Converters.Add(new NullableDateOnlyJsonConverter());

        DateOnly? d = new DateOnly(2025, 8, 14);
        var json = JsonSerializer.Serialize(d, opts);
        json.Should().Be("\"2025-08-14\"");

        var back = JsonSerializer.Deserialize<DateOnly?>(json, opts);
        back.Should().Be(d);

        DateOnly? n = null;
        JsonSerializer.Serialize(n, opts).Should().Be("null");
    }
}
