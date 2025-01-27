using System.ComponentModel.DataAnnotations;
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Commands;

public record RegisterUserCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
    public string Password { get; init; }

    public RegisterUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}