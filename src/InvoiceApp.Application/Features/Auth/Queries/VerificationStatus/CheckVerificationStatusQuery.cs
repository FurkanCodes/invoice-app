using MediatR;

public class CheckVerificationStatusQuery : IRequest<ApiResponse<object>>
{
    public Guid UserId { get; }

    public CheckVerificationStatusQuery(Guid userId)
    {
        UserId = userId;
    }
}
