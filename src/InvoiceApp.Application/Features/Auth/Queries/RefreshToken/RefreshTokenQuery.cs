using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries.RefreshToken;
public record RefreshTokenQuery : IRequest<AuthResponseDto>;
