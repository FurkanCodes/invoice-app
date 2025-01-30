using InvoiceApp.Application.Features.EmailVerification.Commands;
using InvoiceApp.Application.Interfaces;
using MediatR;

public class VerifyEmailWithTokenCommandHandler(IEmailService emailService) : IRequestHandler<VerifyEmailWithTokenCommand, ApiResponse<object>>
{
    private readonly IEmailService _emailService = emailService;

    public async Task<ApiResponse<object>> Handle(VerifyEmailWithTokenCommand request, CancellationToken cancellationToken)
    {
        return await _emailService.VerifyEmailWithToken(request.Token);
    }
}