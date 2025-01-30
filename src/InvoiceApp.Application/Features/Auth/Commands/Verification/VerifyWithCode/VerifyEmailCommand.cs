using InvoiceApp.Application.Features.Auth.DTOs;
using MediatR;

namespace InvoiceApp.Application.Features.EmailVerification.Commands
{
    public class VerifyEmailWithCodeCommand : IRequest<ApiResponse<object>>  // Make sure it implements IRequest
    {
        public string? Code { get; set; }

    }

}