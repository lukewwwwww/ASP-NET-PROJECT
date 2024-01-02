namespace WebApp_ShoppingCart.Models
{
	public class CartItem : Product
	{
		public string? CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateOnly Date { get; set; }

		// Used in ShopController for AddToCart()
		public static CartItem ConvertToCartItem(Product product, string customerId)
		{
			CartItem cartItem = new CartItem
			{
				Id = product.Id,				
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				ImageUrl = product.ImageUrl,
				CustomerId = customerId,
				Quantity = 1,
				Date = DateOnly.FromDateTime(DateTime.Today)
			};
			return cartItem;
		}
	}
}
