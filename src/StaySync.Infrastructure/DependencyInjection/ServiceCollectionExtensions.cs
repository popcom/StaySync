using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaySync.Domain.Interfaces;
using StaySync.Domain.Interfaces.Repositories;
using StaySync.Infrastructure.Persistence;
using StaySync.Infrastructure.Repositories;
using StaySync.Infrastructure.UnitOfWork;
using StaySync.Application.Interfaces.Read;
using StaySync.Infrastructure.Read;

namespace StaySync.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqlConnectionString)
    {
        // Register Dapper handlers once
        DapperTypeHandlers.Register();

        services.AddDbContext<StaySyncDbContext>(opt =>
        {
            opt.UseSqlServer(sqlConnectionString, sql =>
            {
                sql.EnableRetryOnFailure(5);
                sql.MigrationsAssembly(typeof(StaySyncDbContext).Assembly.FullName);
            });
            opt.EnableSensitiveDataLogging(false);
        });

        // Repositories
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ITravelGroupRepository, TravelGroupRepository>();
        services.AddScoped<ITravellerRepository, TravellerRepository>();
        services.AddScoped<IAssignmentsRepository, AssignmentsRepository>();

        // UoW
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Read model (Dapper)
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(sqlConnectionString));
        services.AddScoped<IRoomQueries, RoomQueries>();
        services.AddScoped<IGroupQueries, GroupQueries>();

        return services;
    }
}
