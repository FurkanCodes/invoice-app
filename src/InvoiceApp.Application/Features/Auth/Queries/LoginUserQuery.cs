using System;
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries;

public class LoginUserQuery : IRequest<AuthResponseDto>
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
