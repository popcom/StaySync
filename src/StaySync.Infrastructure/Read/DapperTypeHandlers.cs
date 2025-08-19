using System.Data;
using Dapper;

namespace StaySync.Infrastructure.Read;

internal static class DapperTypeHandlers
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;
        SqlMapper.AddTypeHandler(new DateOnlyHandler());
        SqlMapper.AddTypeHandler(new NullableDateOnlyHandler());
        _registered = true;
    }

    private sealed class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }
        public override DateOnly Parse(object value) => value switch
        {
            DateTime dt => DateOnly.FromDateTime(dt),
            DateTimeOffset dto => DateOnly.FromDateTime(dto.DateTime),
            string s => DateOnly.Parse(s),
            _ => throw new DataException($"Cannot convert {value?.GetType().FullName ?? "null"} to DateOnly.")
        };
    }

    private sealed class NullableDateOnlyHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly? value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.HasValue
                ? value.Value.ToDateTime(TimeOnly.MinValue)
                : DBNull.Value;
        }
        public override DateOnly? Parse(object value) => value switch
        {
            null => null,
            DBNull => null,
            DateTime dt => DateOnly.FromDateTime(dt),
            DateTimeOffset dto => DateOnly.FromDateTime(dto.DateTime),
            string s => DateOnly.Parse(s),
            _ => throw new DataException($"Cannot convert {value.GetType().FullName} to DateOnly?.")
        };
    }
}
