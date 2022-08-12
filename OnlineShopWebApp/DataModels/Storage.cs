using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.DataModels
{
    public partial class Storage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Product? Product { get; set; } = null!;
    }
}
