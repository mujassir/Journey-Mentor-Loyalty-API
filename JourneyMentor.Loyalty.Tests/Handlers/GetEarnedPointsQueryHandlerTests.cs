using JourneyMentor.Loyalty.Application.Features.Points.Handlers;
using JourneyMentor.Loyalty.Application.Features.Points.Queries;
using JourneyMentor.Loyalty.Infrastructure.Redis;
using JourneyMentor.Loyalty.Persistence.Repositories.Interfaces;
using Moq;

namespace JourneyMentor.Loyalty.Tests
{
    public class GetEarnedPointsQueryHandlerTests
    {
        private readonly Mock<IPointsRepository> _repoMock;
        private readonly Mock<IRedisCacheService> _cacheMock;

        public GetEarnedPointsQueryHandlerTests()
        {
            _repoMock = new Mock<IPointsRepository>();
            _cacheMock = new Mock<IRedisCacheService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnPointsFromCache_IfCacheExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedPoints = 100;

            _cacheMock.Setup(x => x.GetAsync<int?>(It.Is<string>(key => key == $"user_points_{userId}")))
                      .ReturnsAsync(expectedPoints);

            var handler = new GetEarnedPointsQueryHandler(_repoMock.Object, _cacheMock.Object);

            // Act
            var result = await handler.Handle(new GetEarnedPointsQuery(userId), CancellationToken.None);

            // Assert
            Assert.Equal(expectedPoints, result);

            _repoMock.Verify(x => x.GetTotalPointsAsync(It.IsAny<Guid>(), default), Times.Never);
            _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<int>(), null), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldQueryDatabaseAndSetCache_IfCacheMiss()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedPoints = 150;

            _cacheMock.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                      .ReturnsAsync((int?)null);

            _repoMock.Setup(x => x.GetTotalPointsAsync(userId, default))
                     .ReturnsAsync(expectedPoints);

            _cacheMock.Setup(x => x.SetAsync(It.IsAny<string>(), expectedPoints, null))
                      .Returns(Task.CompletedTask);

            var handler = new GetEarnedPointsQueryHandler(_repoMock.Object, _cacheMock.Object);

            // Act
            var result = await handler.Handle(new GetEarnedPointsQuery(userId), CancellationToken.None);

            // Assert
            Assert.Equal(expectedPoints, result);

            _repoMock.Verify(x => x.GetTotalPointsAsync(userId, default), Times.Once);
            _cacheMock.Verify(x => x.SetAsync($"user_points_{userId}", expectedPoints, null), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_IfUserHasNoPoints()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _cacheMock.Setup(x => x.GetAsync<int?>(It.IsAny<string>()))
                      .ReturnsAsync((int?)null);

            _repoMock.Setup(x => x.GetTotalPointsAsync(userId, default))
                     .ReturnsAsync(0);

            _cacheMock.Setup(x => x.SetAsync(It.IsAny<string>(), 0, null))
                      .Returns(Task.CompletedTask);

            var handler = new GetEarnedPointsQueryHandler(_repoMock.Object, _cacheMock.Object);

            // Act
            var result = await handler.Handle(new GetEarnedPointsQuery(userId), CancellationToken.None);

            // Assert
            Assert.Equal(0, result);

            _repoMock.Verify(x => x.GetTotalPointsAsync(userId, default), Times.Once);
            _cacheMock.Verify(x => x.SetAsync($"user_points_{userId}", 0, null), Times.Once);
        }
    }
}