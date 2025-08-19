using StaySync.Application.Interfaces;

namespace StaySync.Application.Features.Rooms.Commands.MoveTraveller;

public sealed record MoveTravellerCommand(MoveTravellerRequest Request)
    : ICommand<MoveTravellerResult>;
