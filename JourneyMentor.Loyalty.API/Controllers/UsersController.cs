using JourneyMentor.Loyalty.API.Common.Responses;
using JourneyMentor.Loyalty.Application.Features.Points.Commands;
using JourneyMentor.Loyalty.Application.Features.Points.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JourneyMentor.Loyalty.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet("{id}/points")]
        public async Task<IActionResult> GetUserPoints(Guid id)
        {
            try
            {
                var query = new GetEarnedPointsQuery(id);
                var totalPoints = await _mediator.Send(query);
                return Ok(new { UserId = id, Points = totalPoints });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving points", Details = ex.Message });
            }
        }

        [HttpPost("{id}/earn")]
        public async Task<IActionResult> EarnPoints(Guid id, [FromBody] int points)
        {
            if (points <= 0)
            {
                _logger.LogWarning("Attempted to earn non-positive points: {Points} for user {UserId}", points, id);
                return BadRequest("Points must be greater than zero.");
            }

            try
            {
                var username = User.FindFirst("preferred_username")?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Invalid user attempted to earn points for user ID {UserId}", id);
                    return Unauthorized("User identity is missing.");
                }

                var command = new EarnPointsCommand(id, points, username);
                await _mediator.Send(command);

                return Ok(new ApiResponse<object>(null, $"{points} Points earned successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error earning points for user {UserId} by {Username}", id, User.Identity?.Name ?? "Unknown");
                return StatusCode(500, "An error occurred while earning points.");
            }
        }
    }
}
