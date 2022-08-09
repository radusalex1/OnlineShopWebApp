using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.DataModels
{
    public partial class Client
    {
        public Client()
        {
            Orders = new HashSet<Order>();   
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public int GenderId { get; set; }

        public virtual Gender? Gender { get; set; } 
        public virtual ICollection<Order> Orders { get; set; }
    }
}
