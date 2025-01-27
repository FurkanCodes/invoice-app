using System;
using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries;

public record LoginUserQuery(string Email, string Password) : IRequest<AuthResponseDto>;