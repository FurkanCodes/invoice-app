using FluentEmail.Core;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
        var verificationLink = $"{config["BaseUrl"]}/verify-email-with-token?token={WebUtility.UrlEncode(token)}";
        var template = BuildEmailTemplate(verificationLink, code);

        var emailResponse = await fluentEmail
            .To(user.Email)
            .Subject("Complete Your Registration")
            .Body(template, true)
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
        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2 style='color: #2d3748;'>Verify Your Email</h2>
            <div style='margin: 20px 0; text-align: center;'>
                <a href='{link}' style='button-style'>Verify Automatically</a>
            </div>
            <div style='border-top: 1px solid #e2e8f0; padding-top: 20px;'>
                <p>Or use code: <strong>{code}</strong></p>
                <small>Expires: {DateTime.UtcNow.AddHours(24):MMM dd, yyyy HH:mm}</small>
            </div>
        </div>";

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

    private static void UpdateVerificationStatus(EmailVerification verification, SendResponse response)
    {
        verification.Status = response.Successful
            ? EmailVerificationStatus.Sent
            : EmailVerificationStatus.Failed;
    }




    private static ApiResponse<object> HandleEmailResponse(SendResponse response) => new()
    {
        IsSuccess = response.Successful,
        StatusCode = response.Successful ? HttpStatusCode.OK : HttpStatusCode.BadGateway,
        Message = response.Successful ? "Email sent" : $"Failed: {string.Join(", ", response.ErrorMessages)}"
    };

    private static ApiResponse<object> HandleEmailError(Exception ex) => new()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.InternalServerError,
        Message = $"Email error: {ex.Message}"
    };
}
