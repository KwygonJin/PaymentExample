using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface IPaymentRepository
   {
        Task<IEnumerable<Payment>> GetPaymentsAsync();
        Task<Payment> GetPaymentAsync(int paymentId);
        Task<Payment> GetPaymentBySessionAsync(string sessionId);
        void UpdatePayment(Payment payment);
        Task<Payment> AddPaymentAsync(Payment payment);
    }
}

