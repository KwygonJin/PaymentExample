using Microsoft.EntityFrameworkCore;
using PaymentExample.Data;
using PaymentExample.Interfaces.IRepository;

namespace PaymentExample.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<Price> _db;

        public PriceRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<Price>();
        }

        public async Task<Price> AddPriceAsync(Price price)
        {
            await _db.AddAsync(price);
            return price;
        }

        public async Task DeletePriceAsync(int priceId)
        {
            var price = await _db.FindAsync(priceId);
            if (price != null)
                _db.Remove(price);
        }

        public async Task<IEnumerable<Price>> GetPricesAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        public async Task<Price> GetPriceAsync(int priceId)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.Id == priceId);
        }

        public void UpdatePrice(Price price)
        {
            _db.Attach(price);
            _context.Entry(price).State = EntityState.Modified;
        }

        public async Task<Price> GetPriceByLookUpAsync(string lookupKey)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.LookupKey == lookupKey);
        }
    }
}
