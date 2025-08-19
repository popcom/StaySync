using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StaySync.Application.CQRS;
using StaySync.Application.Features.Groups.Queries.GetGroupRooms;
using StaySync.Application.Features.Rooms.Commands.MoveTraveller;
using StaySync.Application.Features.Rooms.Queries.GetRoomByCode;
using StaySync.Application.Features.Rooms.Queries.GetRoomsToBeOccupiedToday;
using StaySync.Application.Interfaces;

namespace StaySync.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Dispatcher
        services.AddScoped<IDispatcher, Dispatcher>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetRoomsToBeOccupiedTodayQuery, TodayOccupancyDto>, GetRoomsToBeOccupiedTodayHandler>();
        services.AddScoped<IQueryHandler<GetRoomByCodeQuery, RoomDetailsDto>, GetRoomByCodeHandler>();
        services.AddScoped<IQueryHandler<GetGroupRoomsQuery, GroupRoomsDto>, GetGroupRoomsHandler>();

        // Command handlers
        services.AddScoped<ICommandHandler<MoveTravellerCommand, MoveTravellerResult>, MoveTravellerHandler>();

        // Validators
        services.AddScoped<IValidator<GetRoomByCodeQuery>, GetRoomByCodeValidator>();
        services.AddScoped<IValidator<GetGroupRoomsQuery>, GetGroupRoomsValidator>();
        services.AddScoped<IValidator<MoveTravellerCommand>, MoveTravellerValidator>();

        return services;
    }
}
