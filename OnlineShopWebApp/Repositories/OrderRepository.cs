using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ShopContext shopContext) : base(shopContext)
        {
        }

        public async Task<bool> Add(Order objectToAdd)
        {
            await _shopContext.Orders.AddAsync(objectToAdd); 

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(int? id)
        {
            var order = await _shopContext.Orders.FindAsync(id);

            if (order == null)
            {
                return false;
            }

            _shopContext.Orders.Remove(order);

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<Order?> Get(int? id)
        {
            return await _shopContext.Orders
                .Include(val => val.Client)
                .ThenInclude(c => c.Gender)
                .FirstOrDefaultAsync(val => val.Id == id);
        }

        public async Task<List<Order>> GetAll()
        {
            return await _shopContext.Orders
                 .Include(val => val.Client)
                 .ThenInclude(c => c.Gender)
                 .ToListAsync();
        }

        public bool IfExists(int id)
        {
            return _shopContext.Orders.Any(p => p.Id == id);
        }

        public async Task<bool> Update(Order objectToUpdate)
        {
             _shopContext.Orders.Update(objectToUpdate);

            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
