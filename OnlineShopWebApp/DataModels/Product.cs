using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.DataModels
{
    public partial class Product
    {
        public Product()
        {
            OrderedProducts = new HashSet<OrderedProduct>();
            Storages = new HashSet<Storage>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? Decription { get; set; }

        public virtual ICollection<OrderedProduct> OrderedProducts { get; set; }
        public virtual ICollection<Storage> Storages { get; set; }
    }
}
