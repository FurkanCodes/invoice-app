// GetCustomerByIdHandler.cs
using InvoiceApp.Application.Common.Interfaces.Repositories;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

using InvoiceApp.Domain.Entities;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, ApiResponse<Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ApiResponse<Customer>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer == null || customer.IsDeleted)
        {
            return new ApiResponse<Customer>
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Message = "Customer not found."
            };
        }

        // Manual Mapping:  Customer -> CustomerDto
        var customerDto = new Customer
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            // ... map other properties as needed ...
            // Example if you have a Phone property:
            // Phone = customer.Phone,
        };

        return new ApiResponse<Customer>
        {
            IsSuccess = true,
            StatusCode = System.Net.HttpStatusCode.OK,
            Data = customerDto,
            Message = "Customer retrieved successfully."
        };
    }
}

// GetCustomerByIdQuery.cs (remains the same, no changes needed)
// CustomerDto.cs (remains the same, no changes needed)