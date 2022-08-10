using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IStorageRepository : IBaseOperations<Storage>
    {
        public Task<int> GetQuantityByProductId(int productId);
    }
}
