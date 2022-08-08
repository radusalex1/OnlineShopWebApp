using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class BaseRepository
    {
        public readonly ShopContext _shopContext;

        public BaseRepository(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }
    }
}
