using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class GenderRepository : BaseRepository, IGenderRepository
    {
        public GenderRepository(ShopContext shopContext) : base(shopContext)
        {
        }

        public async Task<Gender?> Get(int? id)
        {
            return await _shopContext.Genders.FirstOrDefaultAsync(val => val.Id == id);
        }

        public async Task<List<Gender>> GetAll()
        {
            return await _shopContext.Genders.ToListAsync();
        }
    }
}
