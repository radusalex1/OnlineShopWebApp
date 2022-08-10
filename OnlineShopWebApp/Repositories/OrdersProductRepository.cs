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
            return await _shopContext.OrderedProducts
                .Include(p => p.Product)
                .Include(o => o.Order)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(val => val.Id == id);
        }


        public async Task<List<OrderedProduct>> GetAll()
        {
            return await _shopContext.OrderedProducts
                .Include(c => c.Product)
                .Include(o => o.Order)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        public async Task<List<Product?>> GetProductsFromOrder(int orderId)
        {
            var result = await _shopContext.OrderedProducts
                .Include(p => p.Product)
                .Where(val => val.OrderId == orderId)
                .Select(val => val.Product)
                .ToListAsync();

            return result;
        }

        public async Task<List<OrderedProduct>> GetProductsWithQuantityFromOrder(int orderId)
        {
            return await  _shopContext.OrderedProducts.Where(val => val.OrderId == orderId).ToListAsync();
        }

        public async Task<int> GetQuantityForProductFromOrder(int orderId, int productId)
        {
            var result = await _shopContext.OrderedProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(val => val.OrderId == orderId && val.ProductId == productId);

            return result.Quantity;
        }

        public bool IfExists(int id)
        {
            return _shopContext.OrderedProducts.Any(e => e.Id == id);
        }

        public async Task<bool> IfExists(int entityId,int orderId, int productId)
        {
            return await _shopContext.OrderedProducts.AnyAsync(val=>val.OrderId == orderId && val.ProductId==productId && val.Id!=entityId);
        }

        public async Task<bool> Update(OrderedProduct objectToUpdate)
        {
             _shopContext.OrderedProducts.Update(objectToUpdate);

            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
