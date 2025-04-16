using JourneyMentor.Loyalty.Domain.Entities;

namespace JourneyMentor.Loyalty.Persistence.Repositories.Interfaces
{
    public interface IPointsRepository
    {
        Task AddPointAsync(Point point, CancellationToken cancellationToken = default);
        Task<int> GetTotalPointsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}