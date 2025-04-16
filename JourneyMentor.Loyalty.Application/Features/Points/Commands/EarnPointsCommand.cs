using MediatR;

namespace JourneyMentor.Loyalty.Application.Features.Points.Commands
{
    public record EarnPointsCommand(Guid UserId, int Points, string Username) : IRequest;
}
