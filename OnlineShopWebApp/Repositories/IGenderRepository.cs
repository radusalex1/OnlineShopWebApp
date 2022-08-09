using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IGenderRepository
    {
        public Task<Gender?> Get(int? id);

        public Task<List<Gender>> GetAll();
    }
}
