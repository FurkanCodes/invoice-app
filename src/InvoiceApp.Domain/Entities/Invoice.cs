using InvoiceApp.Domain.Exceptions;

namespace InvoiceApp.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public string ClientName { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }

    public bool IsDeleted { get; private set; }  
    public DateTime? DeletedAt { get; private set; }  


public void  SoftDelete(Guid invoiceId) {
      IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
}

    // Private constructor for EF Core (we’ll add later)


    private Invoice() { }

     public Invoice(string clientName, decimal amount, DateTime dueDate)
    {
        if (string.IsNullOrEmpty(clientName)) throw new DomainException("Client name is required.");
        if (amount <= 0) throw new DomainException("Amount must be positive.");

        Id = Guid.NewGuid();
        ClientName = clientName;
        Amount = amount;
        DueDate = dueDate;
    }
}