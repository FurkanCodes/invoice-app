using InvoiceApp.Application.Features.EmailVerification.Commands;
using InvoiceApp.Application.Interfaces;
using MediatR;

public class VerifyEmailWithCodeCommandHandler(IEmailService emailService) : IRequestHandler<VerifyEmailWithCodeCommand, ApiResponse<object>>
{
    private readonly IEmailService _emailService = emailService;

    public async Task<ApiResponse<object>> Handle(VerifyEmailWithCodeCommand request, CancellationToken cancellationToken)
    {
        return await _emailService.VerifyEmailWithCode(request.Code);
    }
}