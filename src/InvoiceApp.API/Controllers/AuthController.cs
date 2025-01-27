using InvoiceApp.Application.Features.Auth.Commands;
using InvoiceApp.Application.Features.Auth.DTOs;
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
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto request)
    {
        return await _authService.Login(request);
    }
}