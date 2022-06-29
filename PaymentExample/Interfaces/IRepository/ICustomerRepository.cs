using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomer(int customerId);
        Task<Customer> AddCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        void DeleteCustomer(int customerId);
    }
}
