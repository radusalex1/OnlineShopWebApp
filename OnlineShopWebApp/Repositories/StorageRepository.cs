using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class StorageRepository : BaseRepository, IStorageRepository
    {
        public StorageRepository(ShopContext shopContext) : base(shopContext)
        {
        }


        public async Task<bool> Add(Storage objectToAdd)
        {
            await _shopContext.Storages.AddAsync(objectToAdd);

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DecreaseQuantity(int productId, int quantity)
        {
            var product = await _shopContext.Storages.FirstOrDefaultAsync(val => val.ProductId == productId);

            product.Quantity = product.Quantity - quantity;

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(int? id)
        {
            var result = await _shopContext.Storages.FindAsync(id);

            if (result == null)
            {
                return false;
            }

            _shopContext.Storages.Remove(result);

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<Storage?> Get(int? id)
        {
            return await _shopContext.Storages.Include(c => c.Product).FirstOrDefaultAsync(val => val.Id == id);
        }

        public async Task<List<Storage>> GetAll()
        {
            return await _shopContext.Storages.Include(c => c.Product).ToListAsync();
        }

        public async Task<int> GetQuantityByProductId(int productId)
        {
            var product = await _shopContext.Storages.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return 0;
            }
            else
            {
                return product.Quantity;
            }
        }

        public async Task<bool> IfExists(int id)
        {
            return await _shopContext.Storages.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> IncreaseQuantity(int productId, int quantity)
        {
            var product = await _shopContext.Storages.FirstOrDefaultAsync(val => val.ProductId == productId);

            product.Quantity = product.Quantity + quantity;

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Update(Storage objectToUpdate)
        {
            _ = _shopContext.Storages.Update(objectToUpdate);

            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
