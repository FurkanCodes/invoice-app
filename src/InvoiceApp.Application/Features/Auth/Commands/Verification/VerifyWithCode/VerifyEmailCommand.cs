using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.EmailVerification.Commands
{
  public class VerifyEmailWithCodeCommand : IRequest<ApiResponse<object>>
{
    public string? Code { get; set; }
}

    public class VerifyEmailRequest
{
    public string? Code { get; set; }
}


}