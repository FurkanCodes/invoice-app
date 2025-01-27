using System.ComponentModel.DataAnnotations;
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Commands;

public record LogoutCommand : IRequest<AuthResponseDto>;
