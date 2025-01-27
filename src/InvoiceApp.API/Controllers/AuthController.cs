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
public class AuthController( IMediator _mediator) : ControllerBase
{


    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] UserLoginQuery request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        var res = new ApiResponse<object>
        {
            Status = true,
            Message = "Successfully logged out.",
            Data = result
        };
        return Ok(res);

    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken()
    {
        var result = await _mediator.Send(new RefreshTokenQuery());
        return Ok(result);
    }
}