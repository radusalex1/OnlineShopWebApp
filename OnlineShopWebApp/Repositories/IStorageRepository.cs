using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IStorageRepository : IBaseOperations<Storage>
    {
        public Task<int> GetQuantityByProductId(int productId);

        public Task<bool> DecreaseQuantity(int productId, int quantity);

        public Task<bool> IncreaseQuantity(int productId, int quantity);
    }
}
