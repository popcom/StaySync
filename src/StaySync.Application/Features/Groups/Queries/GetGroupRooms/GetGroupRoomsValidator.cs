using FluentValidation;

namespace StaySync.Application.Features.Groups.Queries.GetGroupRooms;

public sealed class GetGroupRoomsValidator : AbstractValidator<GetGroupRoomsQuery>
{
    public GetGroupRoomsValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .Length(6)
            .Matches("^(?!0)[A-Za-z0-9]{6}$")
            .WithMessage("GroupId must be 6 alphanumeric characters, not starting with 0.");
        RuleFor(x => x.GroupId)
            .Must(id => id.Count(char.IsLetter) <= 2)
            .WithMessage("GroupId may contain at most 2 letters.");
    }
}
