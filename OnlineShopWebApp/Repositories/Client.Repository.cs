using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;

namespace OnlineShopWebApp.Repositories
{
    public class ClientRepository : BaseRepository, IClientRepository
    {
        public ClientRepository(ShopContext shopContext) : base(shopContext)
        {
        }

        public async Task<bool> Add(Client objectToAdd)
        {
            await _shopContext.Clients.AddAsync(objectToAdd);

            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> Delete(int? id)
        {
            var result = await _shopContext.Clients.FindAsync(id);

            if (result == null)
            {
                return false;
            }

            _shopContext.Clients.Remove(result);

            await _shopContext.SaveChangesAsync();

            return true;
        }


        public async Task<Client?> Get(int? id)
        {
            return await _shopContext.Clients.Include(c => c.Gender).FirstOrDefaultAsync(val => val.Id == id);
        }


        public async Task<List<Client>> GetAll()
        {
            return await _shopContext.Clients.Include(c => c.Gender).ToListAsync();
        }


        public async Task<bool> IfExists(int id)
        {
            return await _shopContext.Clients.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> IfExists(string phoneNumber)
        {
            return await _shopContext.Clients.AnyAsync(val => val.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IfExists(string phoneNumber, int id)
        {
            var result = await _shopContext.Clients.AsNoTracking().FirstOrDefaultAsync(var => var.PhoneNumber == phoneNumber);

            if (result == null)
                return true;
            if (result.Id == id)
                return true;
            return false;
        }

        public async Task<bool> Update(Client objectToUpdate)
        {
            _ = _shopContext.Clients.Update(objectToUpdate);
            await _shopContext.SaveChangesAsync();

            return true;
        }
    }
}
