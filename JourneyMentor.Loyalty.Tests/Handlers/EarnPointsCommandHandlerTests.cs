using JourneyMentor.Loyalty.Application.Features.Points.Commands;
using JourneyMentor.Loyalty.Domain.Entities;
using JourneyMentor.Loyalty.Infrastructure.Redis;
using JourneyMentor.Loyalty.Persistence.Context;
using JourneyMentor.Loyalty.Persistence.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JourneyMentor.Loyalty.Tests
{
    public class EarnPointsCommandHandlerTests
    {
        private readonly Mock<IPointsRepository> _repoMock;
        private readonly Mock<IRedisCacheService> _cacheMock;
        private readonly EarnPointsCommandHandler _handler;

        public EarnPointsCommandHandlerTests()
        {
            _repoMock = new Mock<IPointsRepository>();
            _cacheMock = new Mock<IRedisCacheService>();
            _handler = new EarnPointsCommandHandler(_repoMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddPointsAndUpdateCache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "test_user";
            var expectedPoints = 150;

            var command = new EarnPointsCommand(userId, expectedPoints, username);

            _repoMock.Setup(x => x.AddPointAsync(It.IsAny<Point>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _repoMock.Setup(x => x.GetTotalPointsAsync(userId, default))
                     .ReturnsAsync(expectedPoints);

            _cacheMock.Setup(x => x.SetAsync<int>($"user_points_{userId}", expectedPoints, It.IsAny<TimeSpan?>()))
                      .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);

            _repoMock.Verify(x => x.AddPointAsync(It.IsAny<Point>(), It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(x => x.GetTotalPointsAsync(userId, default), Times.Once);
            _cacheMock.Verify(x => x.SetAsync<int>($"user_points_{userId}", expectedPoints, It.IsAny<TimeSpan?>()), Times.Once);
        }
    }
}