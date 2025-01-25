using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Auth.DTOs;

public class UserRegisterDto(string email, string password)
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = email;

  [Required]
  [StringLength(100, MinimumLength = 8)]
  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
      ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
  public string Password { get; set; } = password;
}

