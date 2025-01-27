using System;
using InvoiceApp.Application.Features.Auth.DTOs;

namespace InvoiceApp.Application.Interfaces
{

  public interface IAuthService
  {
    Task<ApiResponse<AuthResponseDto>> Register(UserRegisterDto userDto);
    Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto userDto);
  }

}