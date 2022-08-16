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

        public async Task<bool> CancelOrderById(int orderId)
        {      
            var order =  await _shopContext.Orders
               .Include(val => val.Client)
               .ThenInclude(c => c.Gender)
               .FirstOrDefaultAsync(val => val.Id == orderId);

            if(order == null)
            {
                return false;
            }

            order.Canceled = true;

            _shopContext.Orders.Update(order);

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

        public async Task<List<Order>> GetOrdersByClientId(int clientId)
        {
            return await _shopContext.Orders
                .Where(c=>c.ClientId==clientId)
                .ToListAsync();
        }

        public async Task<bool> IfExists(int id)
        {
            return await _shopContext.Orders.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> UnCancelOrderById(int orderId)
        {
            var order = await _shopContext.Orders
               .Include(val => val.Client)
               .ThenInclude(c => c.Gender)
               .FirstOrDefaultAsync(val => val.Id == orderId);

            if (order == null)
            {
                return false;
            }

            order.Canceled = false;

            _shopContext.Orders.Update(order);

            await _shopContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Update(Order objectToUpdate)
        {
             _shopContext.Orders.Update(objectToUpdate);

            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
