using InvoiceApp.Application.Features.Auth.DTOs;
using System.Threading.Tasks;

namespace InvoiceApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task<ApiResponse<object>> SendVerificationEmail(string email, string verificationCode);
    }
}
