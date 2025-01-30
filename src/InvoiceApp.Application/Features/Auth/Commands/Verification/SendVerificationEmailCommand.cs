using MediatR;

namespace InvoiceApp.Application.Features.EmailVerification.Commands
{
    public class SendVerificationEmailCommand : IRequest<ApiResponse<object>>
    {
        public required string Email { get; set; }
    }
}