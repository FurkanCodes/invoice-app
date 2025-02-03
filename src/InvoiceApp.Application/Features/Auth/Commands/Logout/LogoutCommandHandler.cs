using MediatR;
using Microsoft.AspNetCore.Http;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Domain.Exceptions;
using InvoiceApp.Application.Common.Interfaces;

namespace InvoiceApp.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IAuthRepository authRepository) : IRequestHandler<LogoutCommand, Unit>
{


        public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
        {
                var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                        throw new DomainException("Refresh token is empty");
                }

                var storedToken = await authRepository.GetValidRefreshTokenAsync(refreshToken, ct);
                if (storedToken != null)
                {
                        await authRepository.InvalidateRefreshTokenAsync(storedToken, ct);
                        await authRepository.SaveChangesAsync(ct);
                }

                RemoveRefreshTokenCookie();

                return Unit.Value;
        }

        private void RemoveRefreshTokenCookie()
        {
                var cookieOptions = new CookieOptions
                {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1)
                };

                httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken", cookieOptions);
        }
}