using System;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Auth.Commands;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries;

public class LoginUserQueryHandler(IAuthService authService) : IRequestHandler<UserLoginQuery, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(UserLoginQuery query, CancellationToken cancellationToken)
    {
        var dto = new UserLoginDto
        {
            Email = query.Email,
            Password = query.Password
        };

        return await authService.Login(dto);
    }
}
