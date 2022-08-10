using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IOrderRepository : IBaseOperations<Order>
    {
        public Task<bool> CancelOrderById(int orderId);
        public Task<List<Order>> GetOrdersByClientId(int clientId);
    }
}
