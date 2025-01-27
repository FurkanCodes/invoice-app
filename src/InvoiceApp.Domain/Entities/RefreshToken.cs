using System;

namespace InvoiceApp.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsValid { get; set; } = true;
    public Guid UserId { get; set; }
    public required User User { get; set; }
}
