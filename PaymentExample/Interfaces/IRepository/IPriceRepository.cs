using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface IPriceRepository
    {
        Task<IEnumerable<Price>> GetPrices();
        Task<Price> GetPrice(int priceId);
        Task<Price> AddPrice(Price price);
        Task<Price> UpdatePrice(Price price);
        void DeletePrice(int priceId);
    }
}
