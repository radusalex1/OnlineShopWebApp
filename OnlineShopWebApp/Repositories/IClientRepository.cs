using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public interface IClientRepository : IBaseOperations<Client>
    {
        public Task<bool> IfExists(string phoneNumber); 

        public Task<bool> IfExists(string phoneNumber, int id); 
    }
}
