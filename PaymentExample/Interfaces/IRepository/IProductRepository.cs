using PaymentExample.Data;

namespace PaymentExample.Interfaces.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int productId);
        Task<Product> AddProductAsync(Product product);
        void UpdateProduct(Product product);
        Task DeleteProductAsync(int productId);
    }
}
