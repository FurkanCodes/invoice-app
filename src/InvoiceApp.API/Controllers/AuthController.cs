using System.Net;
using InvoiceApp.Application.Features.Auth.Commands;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
using InvoiceApp.Application.Features.EmailVerification.Commands;
using InvoiceApp.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IMediator mediator, IUserService userService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] UserLoginQuery request)
    {
        var result = await _mediator.Send(request);
        return StatusCode((int)result.StatusCode, result);
    }
[HttpPost("logout")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
public async Task<ActionResult<ApiResponse<object>>> Logout()
{
    var result = await _mediator.Send(new LogoutCommand());
    return Ok(new ApiResponse<object>
    {
        StatusCode = HttpStatusCode.OK,
        Message = "Successfully logged out",
        IsSuccess = true,
        Data = result
    });
}

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto? request = null)
    {
        var result = await _mediator.Send(new RefreshTokenQuery(request?.RefreshToken));
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("verification-status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckVerificationStatus()
    {
        try
        {
            var userId = userService.UserId;
            var query = new CheckVerificationStatusQuery(userId);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, new ApiResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "User is not authenticated",
                IsSuccess = false,
                Errors = new List<string> { "UNAUTHORIZED" }
            });
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "An error occurred while checking verification status",
                IsSuccess = false,
                Errors = new List<string> { ex.Message }
            });
        }
    }
    [HttpGet("verify-email-with-token")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> VerifyEmailWithToken([FromQuery] string token)
{
    var command = new VerifyEmailWithTokenCommand { Token = token };
    var result = await _mediator.Send(command);
    return HandleVerificationResult(result);
}

[HttpPost("verify-email-with-code")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> VerifyEmailWithCode([FromQuery] string code)
{
    var command = new VerifyEmailWithCodeCommand { Code = code };
    var result = await _mediator.Send(command);
    return HandleVerificationResult(result);
}

private IActionResult HandleVerificationResult(ApiResponse<object> result)
{
    if (!result.IsSuccess)
    {
        return StatusCode((int)result.StatusCode, result);
    }
    return Ok(result);
}
}




