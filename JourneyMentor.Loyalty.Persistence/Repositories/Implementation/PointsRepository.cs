using JourneyMentor.Loyalty.Domain.Entities;
using JourneyMentor.Loyalty.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JourneyMentor.Loyalty.Persistence.Repositories.Interfaces
{
    public class PointsRepository : IPointsRepository
    {
        private readonly LoyaltyDbContext _context;

        public PointsRepository(LoyaltyDbContext context)
        {
            _context = context;
        }

        public async Task AddPointAsync(Point point, CancellationToken cancellationToken = default)
        {
            _context.Points.Add(point);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetTotalPointsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Points
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Points, cancellationToken);
        }
    }
}