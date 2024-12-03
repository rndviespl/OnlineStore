namespace WebApp2.Controllers
{
    public partial class BrosShopCartController
    {
        public class OrderDetail
        {
            public string? ProductTitle { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
