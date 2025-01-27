using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenQueryHandler(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext context,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenQuery, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RefreshTokenQuery request, CancellationToken ct)
    {
        // 1. Get refresh token from cookie
        var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedAccessException("No refresh token");

        // 2. Validate token against DB

        var storedToken = await context.RefreshTokens.Include(i => i.User).FirstOrDefaultAsync(i => i.Token == refreshToken &&
             i.IsValid &&
             i.ExpiresAt > DateTime.UtcNow, ct);

        if (storedToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        // 3. Generate new access token
        var (accessToken, expiration) = tokenService.GenerateAccessToken(storedToken.User);

        // 4. Rotate refresh token (optional)
        var newRefreshToken = tokenService.GenerateRefreshToken();
        storedToken.IsValid = false;

        await context.RefreshTokens.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = storedToken.User,
        }, ct);

        await context.SaveChangesAsync(ct);

        // 5. Attach new refresh token to cookie
        httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict
            });

        return new AuthResponseDto(true, accessToken, expiration, refreshToken);
    }
}