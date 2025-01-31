using FluentEmail.Core;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Net;
using System.Security.Cryptography;

using FluentEmail.Core.Models;
public class EmailService(
    IFluentEmail fluentEmail,
    AppDbContext dbContext,
    IConfiguration config)
    : IEmailService
{
    private const int MAX_ATTEMPTS = 3;
    private static readonly TimeSpan VERIFICATION_VALIDITY = TimeSpan.FromHours(24);
    private readonly byte[] _hmacKey = Convert.FromBase64String(
        config["Security:HmacKey"] ?? throw new InvalidOperationException("Missing HmacKey configuration")
    );

    public async Task<ApiResponse<object>> SendVerificationEmail(string? email)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return new ApiResponse<object> { /* Error response */ };

        var (verification, token, code) = CreateVerificationRecord(user.Id);

        // Send email immediately after creating record
        var verificationLink = $"{config["BaseUrl"]}/api/auth/verify-email-with-token?token={WebUtility.UrlEncode(token)}";
        var template = BuildEmailTemplate(verificationLink, code);

        var emailResponse = await fluentEmail
            .To(user.Email)
            .Subject("Complete Your Registration")
            .Body(template, isHtml: true)
            .SendAsync();

        verification.Status = emailResponse.Successful
            ? EmailVerificationStatus.Sent
            : EmailVerificationStatus.Failed;

        dbContext.EmailVerifications.Add(verification);
        await dbContext.SaveChangesAsync();

        return new ApiResponse<object>
        {
            IsSuccess = emailResponse.Successful,
            StatusCode = emailResponse.Successful ? HttpStatusCode.OK : HttpStatusCode.BadGateway,
            Message = emailResponse.Successful ? "Verification email sent" : "Failed to send email"
        };
    }



    private async Task<ApiResponse<object>> CompleteVerification(EmailVerification verification)
    {
        try
        {
            verification.Status = EmailVerificationStatus.Success;
            verification.User!.IsEmailVerified = true;
            verification.VerifiedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return new ApiResponse<object>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Email verified successfully",
                Data = new { verification.User.Email }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Verification completion failed: {ex.Message}"
            };
        }
    }
    private (EmailVerification verification, string token, string code) CreateVerificationRecord(Guid userId)
    {
        var verificationToken = GenerateSecureToken();
        var verificationCode = GenerateSecureCode();

        var verification = new EmailVerification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VerificationTokenHash = verificationToken,
            VerificationCodeHash = verificationCode,
            ExpiresAt = DateTime.UtcNow.Add(VERIFICATION_VALIDITY),
            CreatedAt = DateTime.UtcNow,
            Attempts = 0,
            Status = EmailVerificationStatus.Sent
        };

        return (verification, verificationToken, verificationCode);
    }

    public async Task<ApiResponse<object>> VerifyEmailWithToken(string? token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Token is required"
                };

            var verification = await dbContext.EmailVerifications
                .Include(ev => ev.User)
                .FirstOrDefaultAsync(ev =>
                    ev.VerificationTokenHash == token &&
                    ev.Status == EmailVerificationStatus.Sent &&
                    ev.ExpiresAt > DateTime.UtcNow);

            if (verification == null)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid or expired token"
                };
            }

            return await CompleteVerification(verification);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Token verification failed: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<object>> VerifyEmailWithCode(string? code)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Code is required"
                };

            var verification = await dbContext.EmailVerifications
                .Include(ev => ev.User)
                .FirstOrDefaultAsync(ev =>
                    ev.VerificationCodeHash == code &&
                    ev.Status == EmailVerificationStatus.Sent &&
                    ev.ExpiresAt > DateTime.UtcNow);

            if (verification == null)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid or expired code"
                };
            }

            return await CompleteVerification(verification);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Code verification failed: {ex.Message}"
            };
        }
    }
    private static string BuildEmailTemplate(string link, string code) => $@"
       <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f7fa;'>
    <table cellpadding='0' cellspacing='0' border='0' width='100%' style='min-width: 100%; background-color: #f4f7fa;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table cellpadding='0' cellspacing='0' border='0' width='600' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    <tr>
                        <td style='padding: 40px;'>
                            <h1 style='color: #1a202c; font-size: 28px; font-weight: bold; margin: 0 0 20px; text-align: center;'>Verify Your Email</h1>
                            <p style='color: #4a5568; font-size: 16px; line-height: 24px; margin: 0 0 30px; text-align: center;'>Thank you for signing up! Please verify your email address to complete your registration.</p>
                            <table cellpadding='0' cellspacing='0' border='0' width='100%'>
                                <tr>
                                    <td align='center'>
                                        <a href='{link}' style='display: inline-block; background-color: #4299e1; color: #ffffff; font-size: 16px; font-weight: bold; text-decoration: none; padding: 12px 30px; border-radius: 4px; text-align: center; transition: background-color 0.3s ease;'>Verify Automatically</a>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='margin-top: 40px; border-top: 1px solid #e2e8f0; padding-top: 20px;'>
                                <tr>
                                    <td>
                                        <p style='color: #4a5568; font-size: 16px; line-height: 24px; margin: 0 0 10px;'>Or use this verification code:</p>
                                        <p style='color: #1a202c; font-size: 24px; font-weight: bold; letter-spacing: 2px; margin: 0 0 20px; text-align: center;'>{code}</p>
                                        <p style='color: #718096; font-size: 14px; margin: 0;'>Expires: {DateTime.UtcNow.AddHours(24):MMM dd, yyyy HH:mm} UTC</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #edf2f7; padding: 20px; border-bottom-left-radius: 8px; border-bottom-right-radius: 8px;'>
                            <p style='color: #718096; font-size: 14px; margin: 0; text-align: center;'>If you didn't request this email, please ignore it or <a href='#' style='color: #4299e1; text-decoration: none;'>contact support</a> if you have any questions.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>";

    private static string GenerateSecureCode()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 999999)
            .ToString("D6");
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

}
