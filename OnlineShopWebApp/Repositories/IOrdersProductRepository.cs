using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IOrdersProductRepository : IBaseOperations<OrderedProduct>
    {
        public Task<List<Product?>> GetProductsForOrder(int orderId);

        public Task<bool> IfExists(int entityId,int orderId, int productId);

        public Task<List<OrderedProduct>> GetProductsWithQuantityFromOrder(int orderId);

        public Task<int> GetQuantityForProductFromOrder(int orderId,int productId);
    }
}
