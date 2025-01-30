using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Domain.Entities;
using System.Threading.Tasks;

namespace InvoiceApp.Application.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a verification email to the specified email address
        /// </summary>
        Task<ApiResponse<object>> SendVerificationEmail(string? email);

        /// <summary>
        /// Verifies a user's email using either a token or verification code
        /// </summary>
        Task<ApiResponse<object>> VerifyEmailWithToken(string? token);

        Task<ApiResponse<object>> VerifyEmailWithCode(string? code);


    }
}