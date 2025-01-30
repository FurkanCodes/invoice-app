using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.EmailVerification.Commands
{
    public class VerifyEmailCommand : IRequest<ApiResponse<object>>  // Make sure it implements IRequest
    {
        public string? Token { get; set; }
        public string? Code { get; set; }
    }

}