using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface IPriceRepository
    {
        Task<IEnumerable<Price>> GetPricesAsync();
        Task<Price> GetPriceAsync(int priceId);
        Task<Price> GetPriceByLookUpAsync(string lookupKey);
        Task<Price> AddPriceAsync(Price price);
        void UpdatePrice(Price price);
        Task DeletePriceAsync(int priceId);
    }
}
