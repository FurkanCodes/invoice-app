// Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
{
 
    public async Task<ApiResponse<AuthResponseDto>> Handle(
        RegisterUserCommand command, 
        CancellationToken ct)
    {
        var userDto = new UserRegisterDto(command.Email, command.Password);
        return await authService.Register(userDto);
    }
}
