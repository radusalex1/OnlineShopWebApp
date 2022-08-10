using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(ShopContext shopContext) : base(shopContext)
        {
        }

        public async Task<bool> Add(Product objectToAdd)
        {
            await _shopContext.Products.AddAsync(objectToAdd);

            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> Delete(int? id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            _shopContext.Products.Remove(product);

            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<Product?> Get(int? id)
        {
            return await _shopContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<List<Product>> GetAll()
        {
            return await _shopContext.Products.ToListAsync();
        }


        public bool IfExists(int id)
        {
            return _shopContext.Products.Any(p => p.Id == id);
        }


        public async Task<bool> Update(Product objectToUpdate)
        {
            _shopContext.Products.Update(objectToUpdate);

            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
