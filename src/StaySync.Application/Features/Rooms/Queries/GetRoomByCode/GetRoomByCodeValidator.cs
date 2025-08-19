using FluentValidation;

namespace StaySync.Application.Features.Rooms.Queries.GetRoomByCode;

public sealed class GetRoomByCodeValidator : AbstractValidator<GetRoomByCodeQuery>
{
    public GetRoomByCodeValidator()
    {
        RuleFor(x => x.RoomCode).NotEmpty().Matches("^[0-9]{4}$").WithMessage("RoomCode must be 4 digits.");
    }
}
