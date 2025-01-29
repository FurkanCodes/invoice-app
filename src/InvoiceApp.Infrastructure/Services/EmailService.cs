using FluentEmail.Core;
using Quartz;
using System.Net;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using System.Threading.Tasks;

public class EmailService(IFluentEmail fluentEmail) : IEmailService, IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.MergedJobDataMap;
        string email = jobData.GetString("Email")!;
        string verificationCode = jobData.GetString("VerificationCode")!;

        await SendVerificationEmail(email, verificationCode);
    }

    public async Task<ApiResponse<object>> SendVerificationEmail(string email, string verificationCode)
    {
        try
        {
            var emailResponse = await fluentEmail
                .To(email)
                .Subject("Welcome to InvoiceApp - Verify Your Email")
                .Body($@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2 style='color: #2d3748;'>Welcome to InvoiceApp!</h2>
                        <p style='font-size: 16px; color: #4a5568;'>
                            Thank you for registering. Please verify your email address.
                        </p>
                        <div style='background: #f7fafc; padding: 20px; border-radius: 8px; margin-top: 20px;'>
                            <p style='margin: 0; font-weight: 600; color: #2d3748;'>
                                Verification Code: 
                                <a style='color: #3182ce;'>{verificationCode}</span>
                            </p>
                        </div>
                    </div>
                ", isHtml: true)
                .SendAsync();

            return new ApiResponse<object>
            {
                IsSuccess = emailResponse.Successful,
                StatusCode = emailResponse.Successful ? HttpStatusCode.OK : HttpStatusCode.BadGateway,
                Message = emailResponse.Successful
                    ? "Verification email sent successfully"
                    : $"Email failed: {string.Join(", ", emailResponse.ErrorMessages)}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Email sending failed: {ex.Message}"
            };
        }
    }
}
