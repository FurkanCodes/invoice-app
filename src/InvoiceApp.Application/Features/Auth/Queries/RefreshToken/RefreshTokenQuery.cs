using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
public record RefreshTokenQuery : IRequest<ApiResponse<AuthResponseDto>>
{
    public string? BodyToken { get; init; }

    public RefreshTokenQuery() { }

    public RefreshTokenQuery(string? bodyToken)
    {
        BodyToken = bodyToken;
    }
}