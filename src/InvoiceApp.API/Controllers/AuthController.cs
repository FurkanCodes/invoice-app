using InvoiceApp.Application.Features.Auth.Commands;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Auth.Queries;
using InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
using InvoiceApp.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// API/Controllers/AuthController.cs
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IMediator _mediator) : ControllerBase
{
    private readonly IAuthService _authService = authService;


    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginQuery request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        var res = new ApiResponse<object>
        {
            Success = true,
            Message = "Successfully logged out.",
            Data = result
        };
        return Ok(res);

    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        var result = await _mediator.Send(new RefreshTokenQuery());
        return Ok(result);
    }
}