using InvoiceApp.Application.Features.Auth.Commands;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="command">User registration details</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Login with existing credentials
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] UserLoginQuery request)
    {
        return await _mediator.Send(request);
    }

    /// <summary>
    /// Logout the current user
    /// </summary>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        var response = new ApiResponse<object>
        {
            Status = true,
            Message = "Successfully logged out.",
            Data = result
        };
        return Ok(response);
    }

    /// <summary>
    /// Refresh the access token using refresh token from cookie
    /// </summary>
    /// <returns>New authentication tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]

    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken(
    [FromBody] RefreshTokenRequestDto? request = null)
    {
        var result = await _mediator.Send(new RefreshTokenQuery(request?.RefreshToken));
        return Ok(result);
    }




    /// <summary>
    /// Test endpoint that requires authentication
    /// </summary>
    /// <returns>Success message if authenticated</returns>
    [HttpGet("protected")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<string> Protected()
    {
        var userId = User.FindFirst("uid")?.Value;
        return Ok($"Protected resource accessed by user: {userId}");
    }
}
