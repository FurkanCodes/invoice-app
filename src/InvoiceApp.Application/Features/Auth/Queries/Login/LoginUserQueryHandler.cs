using System;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using MediatR;

namespace InvoiceApp.Application.Features.Auth.Queries;

public class LoginUserQueryHandler(IAuthService authService) : IRequestHandler<LoginUserQuery, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginUserQuery query, CancellationToken cancellationToken)
    {
        var dto = new UserLoginDto
        {
            Email = query.Email,
            Password = query.Password
        };

        return await authService.Login(dto);
    }
}
