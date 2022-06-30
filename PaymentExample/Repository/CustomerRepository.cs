using Microsoft.EntityFrameworkCore;
using PaymentExample.Data;
using PaymentExample.Interfaces.IRepository;

namespace PaymentExample.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<Customer> _db;

        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<Customer>();
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            await _db.AddAsync(customer);
            return customer;
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _db.FindAsync(customerId);
            if (customer != null)
                _db.Remove(customer);
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.Id == customerId);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.Email == email);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        public void UpdateCustomer(Customer customer)
        {
            _db.Attach(customer);
            _context.Entry(customer).State = EntityState.Modified;
        }
    }
}
