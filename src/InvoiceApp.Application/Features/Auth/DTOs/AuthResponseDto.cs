using System;

namespace InvoiceApp.Application.Features.Auth.DTOs;

public class AuthResponseDto
{
  public string Token { get; set; } = string.Empty;
  public DateTime Expiration { get; set; }
}