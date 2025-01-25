// Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
  private readonly IAuthService _authService = authService;

  public async Task<AuthResponseDto> Handle(
      RegisterUserCommand request,
      CancellationToken cancellationToken)
  {
    var userDto = new UserRegisterDto(request.Email, request.Password);
    return await _authService.Register(userDto);
  }
}