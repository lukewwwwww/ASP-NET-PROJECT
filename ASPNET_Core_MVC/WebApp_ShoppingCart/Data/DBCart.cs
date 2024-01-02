using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using WebApp_ShoppingCart.Models;

namespace WebApp_ShoppingCart.Data
{
	public class DBCart
	{
		// (For View) Returns list of cart items by username
		public static List<CartItem> GetCartItems(string username)
		{
			if (String.IsNullOrEmpty(username)) { return null; }

			List<CartItem> cartItems = new List<CartItem>();
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT P.ProductID, P.ProductName, P.Description, P.Price, C.CustomerID, C.Quantity 
							   FROM ProductList P, Cart C
							   WHERE P.ProductID = C.ProductID AND C.CustomerID = @CustomerId";

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@CustomerId", username);
				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					CartItem cartItem = new CartItem()
					{
						Id = (string)reader["ProductID"],
						Name = (string)reader["ProductName"],
						Description = (string)reader["Description"],
						Price = (decimal)reader["Price"],
						ImageUrl = $"/img/{(string)reader["ProductID"]}.png",
						CustomerId = (string)reader["CustomerId"],
						Quantity = (int)reader["Quantity"]
					};
					cartItems.Add(cartItem);
				}
				return cartItems;
			}
		}

		// (For view) Returns quantity of items in cart
		public static int GetNonUniqueCount(string username)
		{
			if (String.IsNullOrEmpty(username)) { return 0; }
			List<CartItem> cartItems = GetCartItems(username);
			
			int count = 0;

			foreach (CartItem cartItem in cartItems)
			{
				count += cartItem.Quantity;
			}
			return count;
		}

		// Checks if item is already in cart
		// Helper function for AddToCart()
		public static bool IsItemInCart(string id, string username)
		{
			if (String.IsNullOrEmpty(id)) { return false; }

			int count;
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT COUNT(*) FROM Cart WHERE ProductID = @ProductId AND CustomerID = @CustomerId";

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@ProductId", id);
				cmd.Parameters.AddWithValue("@CustomerId", username);

				count = (int)cmd.ExecuteScalar();
			}
			return (count > 0);
		}

		// Handles adding new item to Cart or increasing quantity
		public static void AddToCart(CartItem cartItem)
		{
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();

				if (!IsItemInCart(cartItem.Id, cartItem.CustomerId))
				{
					// If Item does not exist in Cart, add to cart
					string sql = @"INSERT INTO Cart(ProductID, Quantity, CustomerID, Date) 
								   VALUES(@ProductId, @ProductQuantity, @CustomerId, @Date);";
					SqlCommand cmd = new SqlCommand(sql, conn);

					cmd.Parameters.AddWithValue("@ProductId", cartItem.Id);
					cmd.Parameters.AddWithValue("@ProductQuantity", cartItem.Quantity);
					cmd.Parameters.AddWithValue("@CustomerId", cartItem.CustomerId);
					cmd.Parameters.AddWithValue("@Date", cartItem.Date);
					cmd.ExecuteNonQuery();
				}
				else
				{
					// If Item already exists in Cart, update quantity
					string updateSql = @"UPDATE Cart SET Quantity = Quantity + 1 
                             WHERE ProductID = @ProductId";

					SqlCommand cmd2 = new SqlCommand(updateSql, conn);
					cmd2.Parameters.AddWithValue("@ProductId", cartItem.Id);
					cmd2.ExecuteNonQuery();
				}
			}
		}

		// Update item quantity in cart
		public static void UpdateQuantity(string productId, int quantity, string userId)
		{
			if (quantity <= 0) { RemoveItem(productId, userId); return; }

			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string updateSql = @"UPDATE Cart SET Quantity = @Quantity 
                             WHERE ProductID = @ProductId AND CustomerID = @CustomerId";

				SqlCommand cmd = new SqlCommand(updateSql, conn);
				cmd.Parameters.AddWithValue("@Quantity", quantity);
				cmd.Parameters.AddWithValue("@ProductId", productId);
				cmd.Parameters.AddWithValue("@CustomerId", userId);
				cmd.ExecuteNonQuery();
			}
		}

		// Remove item from cart
		public static void RemoveItem(string productId, string userId)
		{
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string deleteSql = @"DELETE FROM Cart
                             WHERE ProductID = @ProductId AND CustomerID=@CustomerId";

				SqlCommand cmd = new SqlCommand(deleteSql, conn);
				cmd.Parameters.AddWithValue("@ProductId", productId);
				cmd.Parameters.AddWithValue("@CustomerId", userId);
				cmd.ExecuteNonQuery();
			}
		}

		// Transfers all items from sessionId's cart to userId
		public static void UpdateCartUser(string sessionId, string userId)
		{
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"UPDATE Cart SET CustomerID = @UserId 
                               WHERE CustomerID = @SessionId";

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@SessionId", sessionId);
				cmd.ExecuteNonQuery();
			}
		}
	}
}