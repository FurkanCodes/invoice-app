using System;
using InvoiceApp.Application.Features.Auth.DTOs;

namespace InvoiceApp.Application.Interfaces
{
  public interface IAuthService
  {
    Task<AuthResponseDto> Register(UserRegisterDto userDto);
    Task<AuthResponseDto> Login(UserLoginDto userDto);
  }
}