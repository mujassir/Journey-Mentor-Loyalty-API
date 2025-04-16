using JourneyMentor.Loyalty.Domain.Entities;
using JourneyMentor.Loyalty.Infrastructure.Redis;
using MediatR;
using JourneyMentor.Loyalty.Persistence.Repositories.Interfaces;

namespace JourneyMentor.Loyalty.Application.Features.Points.Commands
{
    public class EarnPointsCommandHandler : IRequestHandler<EarnPointsCommand>
    {
        private readonly IPointsRepository _repo;
        private readonly IRedisCacheService _cache;

        public EarnPointsCommandHandler(IPointsRepository repo, IRedisCacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<Unit> Handle(EarnPointsCommand request, CancellationToken cancellationToken)
        {
            var point = new Point
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Points = request.Points,
                CreatedBy = request.Username,
            };

            await _repo.AddPointAsync(point, cancellationToken);

            // Update Redis cache
            var totalPoints = await _repo.GetTotalPointsAsync(request.UserId);
            await _cache.SetAsync<int>($"user_points_{request.UserId}", totalPoints);

            return Unit.Value;
        }
    }
}
