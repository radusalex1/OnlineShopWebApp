using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    /// <summary>
    /// TODO: maybe change theese names.
    /// </summary>
    public interface IOrderedProductRepository : IBaseOperations<OrderedProduct>
    {
        /// <summary>
        /// Return all the products from an Order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Task<List<Product?>> GetProductsFromOrder(int orderId);

        /// <summary>
        /// Returns a list of OrderedProducts by Order Id.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Task<List<OrderedProduct>> GetProductsWithQuantityFromOrder(int orderId);

        /// <summary>
        /// Returns the quantity of a Product from an Order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Task<int> GetQuantityForProductFromOrder(int orderId, int productId);

        public Task<bool> IfExists(int entityId, int orderId, int productId);
    }
}
