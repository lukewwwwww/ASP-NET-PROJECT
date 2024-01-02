namespace WebApp_ShoppingCart.Models
{
	public class Purchase : Product
	{
		public string? CustomerId { get; set; }
		public int? OrderId { get; set; }
		public List<string>? ActivationCode { get; set; }
		public DateOnly? Date { get; set; }

		// Returns TRUE items of the same ProductId are part of the same OrderId
		public static bool ComparePurchase(Purchase p1, Purchase p2)
		{
			if (p1.Id == p2.Id &&								// Same ProductId
				p1.OrderId == p2.OrderId &&						// Bought in the same OrderId
				p1.ActivationCode != p2.ActivationCode) {		// Are different items
				return true;
			}
			return false;
		}

		public static Purchase ConvertToPurchaseItem(CartItem cartItem, int orderId)
		{
			Purchase purchaseItem = new Purchase
			{
				Id = cartItem.Id,
				Name = cartItem.Name,
				Description = cartItem.Description,
				Price = cartItem.Price,
				ImageUrl = cartItem.ImageUrl,
				CustomerId = cartItem.CustomerId,
				OrderId = orderId,
				ActivationCode = new List<string>() { Guid.NewGuid().ToString() },
				Date = DateOnly.FromDateTime(DateTime.Today)
			};
			return purchaseItem;
		}
	}
}
