using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerAsync(int customerId);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> AddCustomerAsync(Customer customer);
        void UpdateCustomer(Customer customer);
        Task DeleteCustomerAsync(int customerId);
    }
}
