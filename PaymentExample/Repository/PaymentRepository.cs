using Microsoft.EntityFrameworkCore;
using PaymentExample.Data;
using PaymentExample.Interfaces.IRepository;

namespace PaymentExample.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<Payment> _db;

        public PaymentRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<Payment>();
        }
        
        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            await _db.AddAsync(payment);
            return payment;
        }

        public async Task<Payment> GetPaymentAsync(int paymentId)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.Id == paymentId);
        }

        public async Task<Payment> GetPaymentBySessionAsync(string sessionId)
        {
            return await _db.AsNoTracking().FirstOrDefaultAsync(q => q.SessionId == sessionId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        public void UpdatePayment(Payment payment)
        {
            _db.Attach(payment);
            _context.Entry(payment).State = EntityState.Modified;
        }
    }
}
