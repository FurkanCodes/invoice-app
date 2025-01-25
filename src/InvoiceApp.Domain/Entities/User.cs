using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Domain.Entities;

public class User
{
  public Guid Id { get; set; }

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
  public byte[] PasswordHash { get; set; } = [];
  public byte[] PasswordSalt { get; set; } = [];
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
