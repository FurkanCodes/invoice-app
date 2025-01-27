using System;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, IApplicationDbContext context) : IRequestHandler<LogoutCommand, AuthResponseDto>
{

        public async Task<AuthResponseDto> Handle(LogoutCommand request, CancellationToken ct)
        {
                // 1. Get refresh token from cookie
                var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                        return new AuthResponseDto(string.Empty, DateTime.UtcNow, string.Empty);

                // 2. Revoke token in DB
                var storedToken = await context.RefreshTokens
                    .FirstOrDefaultAsync(i => i.Token == refreshToken, ct);

                if (storedToken != null)
                {
                        storedToken.IsValid = false;
                        await context.SaveChangesAsync(ct);
                }

                // 3. Delete cookie with matching options
                var cookieOptions = new CookieOptions
                {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                };

                httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken", cookieOptions);

                return new AuthResponseDto(string.Empty, DateTime.UtcNow, string.Empty);
        }
}
