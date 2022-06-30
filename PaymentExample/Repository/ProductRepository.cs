using Microsoft.EntityFrameworkCore;
using PaymentExample.Data;
using PaymentExample.Interfaces.IRepository;

namespace PaymentExample.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<Product> _db;

        public ProductRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<Product>();
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _db.AddAsync(product);
            return product;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _db.FindAsync(productId);
            if (product != null)
                _db.Remove(product);
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        public void UpdateProduct(Product product)
        {
            _db.Attach(product);
            _context.Entry(product).State = EntityState.Modified;
        }
    }
}
