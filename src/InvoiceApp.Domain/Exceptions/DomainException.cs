using System;

namespace InvoiceApp.Domain.Exceptions;

public class DomainException(string message) : Exception(message)
{
}
