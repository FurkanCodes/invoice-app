// Application/Features/Auth/Commands/RegisterUserCommand.cs
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Commands;

public record RegisterUserCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;