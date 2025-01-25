namespace InvoiceApp.Application.Common.Exceptions;

public class AuthException : Exception
{
  public AuthException(string message) : base(message)
  {
  }
}