using System.Text.Json;
using System.Text.Json.Serialization;

namespace StaySync.WebApi.Serialization;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.Parse(reader.GetString()!);
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format));
}

public sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    private const string Format = "yyyy-MM-dd";
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? (DateOnly?)null : DateOnly.Parse(reader.GetString()!);
    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is null) { writer.WriteNullValue(); return; }
        writer.WriteStringValue(value.Value.ToString(Format));
    }
}
