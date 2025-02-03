using System.Net;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

public class RefreshTokenQueryHandler(
    IHttpContextAccessor httpContextAccessor,
    IAuthRepository authRepository,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenQuery, ApiResponse<AuthResponseDto>>
{
    public async Task<ApiResponse<AuthResponseDto>> Handle(
        RefreshTokenQuery request,
        CancellationToken ct)
    {
        // 1. Get refresh token from cookie
        var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"]
                        ?? request.BodyToken;

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedAccessException("No refresh token");

        // 2. Validate token using repository
        var storedToken = await authRepository.GetValidRefreshTokenAsync(refreshToken, ct)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        // 3. Generate new access token
        var (accessToken, expiration) = tokenService.GenerateAccessToken(storedToken.User);

        // 4. Rotate refresh token
        var newRefreshToken = tokenService.GenerateRefreshToken();

        // Invalidate old token using repository
        await authRepository.InvalidateRefreshTokenAsync(storedToken, ct);

        // Add new refresh token using repository
        await authRepository.AddRefreshTokenAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = storedToken.User,
        }, ct);

        // Save changes in single transaction
        await authRepository.SaveChangesAsync(ct);

        // 5. Attach new refresh token to cookie
        httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict
            });

        return new ApiResponse<AuthResponseDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Refresh token successful",
            IsSuccess = true,
            Data = new AuthResponseDto
            {
                Token = accessToken,
                Expiration = expiration,  // Use actual expiration from token service
                RefreshToken = newRefreshToken  // Return the new refresh token
            }
        };
    }
}