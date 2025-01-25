using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Auth.DTOs;
public class UserLoginDto
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}
