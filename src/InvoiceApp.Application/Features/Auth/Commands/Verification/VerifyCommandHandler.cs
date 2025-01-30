using InvoiceApp.Application.Features.EmailVerification.Commands;
using InvoiceApp.Application.Interfaces;
using MediatR;

public class VerifyEmailCommandHandler(IEmailService emailService) : IRequestHandler<VerifyEmailCommand, ApiResponse<object>>
{
    private readonly IEmailService _emailService = emailService;

    public async Task<ApiResponse<object>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        return await _emailService.VerifyEmail(request.Token, request.Code);
    }
}