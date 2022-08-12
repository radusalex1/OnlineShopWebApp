namespace OnlineShopWebApp.DataModels
{
    public partial class Order
    {
        public Order()
        {
            OrderedProducts = new HashSet<OrderedProduct>();
        }

        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Created { get; set; }
        public double? TotalAmount { get; set; }
        public bool Canceled { get; set; }

        public virtual Client? Client { get; set; } = null!;
        public virtual ICollection<OrderedProduct> OrderedProducts { get; set; }
    }
}
