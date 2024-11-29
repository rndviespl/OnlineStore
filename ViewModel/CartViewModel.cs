namespace WebApp2.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<BrosShopProduct> Products { get; set; } // Список продуктов
    }
}