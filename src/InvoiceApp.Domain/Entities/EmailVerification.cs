using System;

namespace InvoiceApp.Domain.Entities;
public class EmailVerification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public required string VerificationTokenHash { get; set; }
    public required string VerificationCodeHash { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int Attempts { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public EmailVerificationStatus Status { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

public enum EmailVerificationStatus
{
    Pending,
    Sent,
    Failed,
    Success
}