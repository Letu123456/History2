using System.Collections.Generic;

namespace Business.DTO
{
    public class Products
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItems
    {
        public Products Product { get; set; }
        public int Count { get; set; }
    }

    public static class ShoppingCartDto
    {
        public static List<CartItems> ListCart { get; } = new List<CartItems>
        {
            new CartItems
            {
                Product = new Products { Name = "Premium", Price = 20 },
                Count = 1
            },
            
        };
    }
}
