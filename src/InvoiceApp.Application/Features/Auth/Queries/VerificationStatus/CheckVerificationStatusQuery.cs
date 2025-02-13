using MediatR;

public class CheckVerificationStatusQuery : IRequest<ApiResponse<object>>
{
    public string? Email { get; }
    public string? VerificationToken { get; }

    public CheckVerificationStatusQuery(string? email = null, string? verificationToken = null)
    {
        Email = email;
        VerificationToken = verificationToken;
    }
}