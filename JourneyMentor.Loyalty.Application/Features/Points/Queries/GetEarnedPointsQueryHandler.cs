using JourneyMentor.Loyalty.Application.Features.Points.Queries;
using JourneyMentor.Loyalty.Infrastructure.Redis;
using JourneyMentor.Loyalty.Persistence.Repositories.Interfaces;
using MediatR;

namespace JourneyMentor.Loyalty.Application.Features.Points.Handlers
{
    public class GetEarnedPointsQueryHandler : IRequestHandler<GetEarnedPointsQuery, int>
    {
        private readonly IPointsRepository _repo;
        private readonly IRedisCacheService _cache;

        public GetEarnedPointsQueryHandler(IPointsRepository repo, IRedisCacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<int> Handle(GetEarnedPointsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"user_points_{request.UserId}";
            // Try to get from Redis
            var cachedPoints = await _cache.GetAsync<int?>(cacheKey);
            if (cachedPoints.HasValue)
            {
                return cachedPoints.Value;
            }

            // Not in cache, query DB
            var totalPoints = await _repo.GetTotalPointsAsync(request.UserId);

            await _cache.SetAsync(cacheKey, totalPoints);

            return totalPoints;
        }
    }
}
