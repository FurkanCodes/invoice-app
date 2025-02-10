using MediatR;

public class CheckVerificationStatusQueryHandler : IRequestHandler<CheckVerificationStatusQuery, ApiResponse<object>>
{
    private readonly IUserRepository _userRepository;

    public CheckVerificationStatusQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<object>> Handle(CheckVerificationStatusQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return new ApiResponse<object>
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Message = "User not found",
                IsSuccess = false,
                Data = null
            };
        }

        bool isVerified = await _userRepository.IsEmailConfirmedAsync(request.UserId);

        return new ApiResponse<object>
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Message = "Verification status retrieved successfully",
            IsSuccess = true,
            Data = new { IsVerified = isVerified }
        };
    }
}
