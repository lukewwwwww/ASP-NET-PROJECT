using Microsoft.Data.SqlClient;
using WebApp_ShoppingCart.Models;

namespace WebApp_ShoppingCart.Data
{
	public class DBPurchase
	{
		// (For View) Returns list of purchases by username
		public static List<Purchase> GetPurchaseHistory(string username)
		{
			List<Purchase> purchases = new List<Purchase>();

			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT P.ProductID, P.ProductName, P.Description, O.OrderId, O.PurchaseDate, OD.ActivationCode 
                               FROM ProductList P, OrderHeader O, OrderDetails OD
                               WHERE P.ProductID = OD.ProductID AND O.OrderID = OD.OrderID AND O.CustomerID = @Username 
							   ORDER BY OrderId DESC, ProductID";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@Username", username);
				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Purchase purchase = new Purchase()
					{
						Id = (string)reader["ProductId"],
						Name = (string)reader["ProductName"],
						Description = (string)reader["Description"],
						ImageUrl = $"/img/{(string)reader["ProductID"]}.png",
						OrderId = (int)reader["OrderId"],
						ActivationCode = new List<string>(),
						Date = DateOnly.FromDateTime((DateTime)reader["PurchaseDate"])
					};
					purchase.ActivationCode.Add((string)reader["ActivationCode"]);

					// If new Purchase is same as prev Purchase (same ProductID and OrderID)
					// works because of ORDER BY clause in SQL
					if (purchases.Count() != 0 && Purchase.ComparePurchase(purchase, purchases[purchases.Count() - 1]))
					{
						// Add to prev Purchase ActivationCode list
						purchases[purchases.Count() - 1].ActivationCode.Add(purchase.ActivationCode[0]);
					}
					else
					{
						// Insert new entry to list
						purchases.Add(purchase);
					}
				}
			}
			return purchases;
		}

		// For Checkout Action Method
		public static void CheckoutCart(List<CartItem> cartItems, string username)
		{
			if (cartItems == null || cartItems.Count() == 0) { return; }

			int orderId = CreateOrderHeader(username);

			foreach (CartItem item in cartItems)
			{
				for (int i = 0; i < item.Quantity; i++)
				{
					Purchase purchaseItem = Purchase.ConvertToPurchaseItem(item, orderId);
					CreateOrderDetail(purchaseItem);
				}
				DBCart.RemoveItem(item.Id, username);
			}
		}

		// Helper function for CheckoutCart
		// Creates OrderHeader and returns OrderId created
		public static int CreateOrderHeader(string username)
		{
			int orderId;
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"INSERT INTO OrderHeader (CustomerID, PurchaseDate) 
							   VALUES (@CustomerId, @PurchaseDate);
							   SELECT SCOPE_IDENTITY();";
				SqlCommand cmd = new SqlCommand(sql, conn);

				cmd.Parameters.AddWithValue("@CustomerId", username);
				cmd.Parameters.AddWithValue("@PurchaseDate", DateOnly.FromDateTime(DateTime.Today));
				orderId = Convert.ToInt32(cmd.ExecuteScalar());
			}
			return orderId;
		}

		// Helper function for CheckoutCart
		public static void CreateOrderDetail(Purchase purchaseItem)
		{
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"INSERT INTO OrderDetails (OrderID, ProductID, ActivationCode) 
							   VALUES (@OrderId, @ProductId, @ActivationCode);";
				SqlCommand cmd = new SqlCommand(sql, conn);

				cmd.Parameters.AddWithValue("@OrderId", purchaseItem.OrderId);
				cmd.Parameters.AddWithValue("@ProductId", purchaseItem.Id);
				cmd.Parameters.AddWithValue("@ActivationCode", purchaseItem.ActivationCode[0]);
				cmd.ExecuteNonQuery();
			}
		}
	}
}
