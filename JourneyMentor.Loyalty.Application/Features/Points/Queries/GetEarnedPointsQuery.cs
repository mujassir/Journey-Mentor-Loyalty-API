using MediatR;

namespace JourneyMentor.Loyalty.Application.Features.Points.Queries
{
    public record GetEarnedPointsQuery(Guid UserId) : IRequest<int>;
}
