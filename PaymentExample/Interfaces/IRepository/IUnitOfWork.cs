namespace PaymentExample.Interfaces.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Cusotmers { get; }

        IPriceRepository Prices { get; }

        IProductRepository Products { get; }

        IPaymentRepository Payments { get; }

        Task SaveAsync();
    }
}
