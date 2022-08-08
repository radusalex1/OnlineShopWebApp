﻿using System;
using System.Collections.Generic;

namespace OnlineShopWebApp.DataModels
{
    public partial class OrderedProduct
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
