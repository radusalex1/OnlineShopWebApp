using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IProductRepository : IBaseOperations<Product>
    {
        /// <summary>
        /// For creating new objects.
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public Task<bool> IfExists(string productName);

        /// <summary>
        /// In case you edit the same current object.
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> IfExists(string productName, int id);
    }
}
