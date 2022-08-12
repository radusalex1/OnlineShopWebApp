using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IProductRepository : IBaseOperations<Product>
    {
        public Task<bool> IfExists(string productName);
    }
}
