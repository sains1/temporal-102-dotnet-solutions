namespace Application.Data;

public interface ICustomerInfoDatabase
{
    Task<CustomerInfo> Get(string customerId);
}

public class InMemoryCustomerInfoDatabase : ICustomerInfoDatabase
{
    public Task<CustomerInfo> Get(string customerId)
    {
        if (!_customers.TryGetValue(customerId, out var customer))
        {
            throw new KeyNotFoundException("customer ID does not exist in the database");
        }

        return Task.FromResult(customer);
    }

    private readonly Dictionary<string, CustomerInfo> _customers = new()
    {
        ["a100"] =
            new CustomerInfo
            {
                CustomerId = "a100",
                Name = "Ana Garcia",
                EmailAddress = "ana@example.com",
                Amount = 500,
                NumberOfPeriods = 10
            },
        ["a101"] = new CustomerInfo
        {
            CustomerId = "a101",
            Name = "Amit Singh",
            EmailAddress = "asignh@exanple.com",
            Amount = 250,
            NumberOfPeriods = 15
        },
        ["a102"] = new CustomerInfo
        {
            CustomerId = "a102",
            Name = "Mary O'Connor",
            EmailAddress = "marymo@example.com",
            Amount = 425,
            NumberOfPeriods = 12
        }
    };
}
