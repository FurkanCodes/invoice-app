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
        <!DOCTYPE html>
        <html>
        <head>
            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
            <!--[if !mso]><!-->
            <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
            <!--<![endif]-->
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        </head>
        <body style=""margin: 0; padding: 0; background-color: #ffffff;"">
            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif;"">
                <tr>
                    <td style=""padding: 20px; background-color: #ffffff;"">
                        <h2 style=""color: #2d3748; margin: 0 0 20px 0;"">Welcome to InvoiceApp!</h2>
                        <p style=""font-size: 16px; color: #4a5568; margin: 0 0 20px 0;"">
                            Thank you for registering. Please verify your email address.
                        </p>
                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background: #f7fafc; border-radius: 8px; padding: 20px;"">
                            <tr>
                                <td style=""text-align: center;"">
                                    <p style=""margin: 0; font-weight: 600; color: #2d3748;"">
                                        Verification Code: 
                                        <span style=""color: #3182ce;"">{verificationCode}</span>
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
    ")
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
