using System.Net;
using InvoiceApp.Domain.Entities;
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
        if (!string.IsNullOrEmpty(request.Email))
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User not found",
                    IsSuccess = false
                };
            }

            return new ApiResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Verification status retrieved successfully",
                IsSuccess = true,
                Data = new { IsVerified = user.IsEmailVerified }
            };
        }
        else if (!string.IsNullOrEmpty(request.VerificationToken))
        {
            var verification = await _userRepository.GetEmailVerificationByTokenAsync(request.VerificationToken);
            if (verification == null)
            {
                return new ApiResponse<object>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid or expired verification token",
                    IsSuccess = false
                };
            }

            return new ApiResponse<object>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Verification status retrieved successfully",
                IsSuccess = true,
                Data = new 
                { 
                    IsVerified = verification.Status == EmailVerificationStatus.Success,
                    Status = verification.Status.ToString()
                }
            };
        }

        return new ApiResponse<object>
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Either email or verification token must be provided",
            IsSuccess = false
        };
    }
}