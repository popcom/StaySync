using FluentValidation;

namespace StaySync.Application.Features.Rooms.Commands.MoveTraveller;

public sealed class MoveTravellerValidator : AbstractValidator<MoveTravellerCommand>
{
    public MoveTravellerValidator()
    {
        RuleFor(x => x.Request.GroupId)
            .NotEmpty().Length(6)
            .Matches("^(?!0)[A-Za-z0-9]{6}$")
            .WithMessage("GroupId must be 6 alphanumeric chars, not starting with 0.")
            .Must(id => id.Count(char.IsLetter) <= 2).WithMessage("GroupId may contain at most 2 letters.");

        RuleFor(x => x.Request.Surname).NotEmpty();
        RuleFor(x => x.Request.FirstName).NotEmpty();
        RuleFor(x => x.Request.FromRoomCode).NotEmpty().Matches("^[0-9]{4}$");
        RuleFor(x => x.Request.ToRoomCode).NotEmpty().Matches("^[0-9]{4}$");
        RuleFor(x => x.Request.AssignedOnDate).NotEmpty();
    }
}
