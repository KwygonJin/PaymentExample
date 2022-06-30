using PaymentExample.Data;
using PaymentExample.Interfaces.IRepository;

namespace PaymentExample.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private ICustomerRepository _customers;
        private IPriceRepository _prices;
        private IProductRepository _products;
        private IPaymentRepository _payments;

        public UnitOfWork(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public ICustomerRepository Cusotmers => _customers ??= new CustomerRepository(_context);

        public IPriceRepository Prices => _prices ??= new PriceRepository(_context);

        public IProductRepository Products => _products ??= new ProductRepository(_context);

        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
