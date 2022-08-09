using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.DataModels
{
    public partial class Gender
    {
        public Gender()
        {
            Clients = new HashSet<Client>();
        }

        public int Id { get; set; }
        public string? GenderType { get; set; } = null!;

        public virtual ICollection<Client> Clients { get; set; }
    }
}
