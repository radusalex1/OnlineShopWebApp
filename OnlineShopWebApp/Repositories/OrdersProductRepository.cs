using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class OrdersProductRepository : BaseRepository, IOrdersProductRepository
    {
        public OrdersProductRepository(ShopContext shopContext) : base(shopContext)
        {
        }


        public async Task<bool> Add(OrderedProduct objectToAdd)
        {
            await _shopContext.OrderedProducts.AddAsync(objectToAdd);
            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> Delete(int? id)
        {
            var result = await _shopContext.OrderedProducts.FindAsync(id);

            if (result == null)
            {
                return false;
            }

            _shopContext.OrderedProducts.Remove(result);

            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<OrderedProduct?> Get(int? id)
        {
            return await _shopContext.OrderedProducts.Include(c=> c.Product).Include(o => o.Order).FirstOrDefaultAsync(val => val.Id == id);
        }


        public async Task<List<OrderedProduct>> GetAll()
        {
            return await _shopContext.OrderedProducts.Include(c => c.Product).Include(o => o.Order).ToListAsync();
        }


        public bool IfExists(int id)
        {
            return _shopContext.OrderedProducts.Any(e => e.Id == id);
        }


        public async Task<bool> Update(OrderedProduct objectToUpdate)
        {
            _ = _shopContext.OrderedProducts.Update(objectToUpdate);
            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
