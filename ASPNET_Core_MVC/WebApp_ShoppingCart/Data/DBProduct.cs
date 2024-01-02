using Microsoft.Data.SqlClient;
using WebApp_ShoppingCart.Models;
using System;
using System.Collections.Generic;

namespace WebApp_ShoppingCart.Data
{
	public class DBProduct
	{
		// (For View) Returns a list of all products from DB
		public static List<Product> GetProducts()
		{
			List<Product> products = new List<Product>();
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT ProductID, ProductName, Price, Description FROM ProductList";
				SqlCommand cmd = new SqlCommand(sql, conn);
				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Product product = new Product()
					{
						Id = (string)reader["ProductID"],
						Name = (string)reader["ProductName"],
						Description = (string)reader["Description"],
						Price = (decimal)reader["Price"],
						ImageUrl = $"/img/{(string)reader["ProductID"]}.png"
					};
					products.Add(product);
				}
			}
			return products;
		}
		
		public static List<bool>? GetProductsFilter(string search)
		{
			List<Product> products = GetProducts();
			List<bool> filter = new List<bool>();
			if (!String.IsNullOrEmpty(search)) { search = search.ToLower(); }

			foreach(Product product in products)
			{
				if (String.IsNullOrEmpty(search) || product.Name.ToLower().Contains(search) || product.Description.ToLower().Contains(search))
				{
					filter.Add(true);	// Show
				} else
				{
					filter.Add(false);	// Hide
				}
			}
			return filter;
		}


		// Returned filtered list of products in DB by search term
		public static List<Product> GetFilteredProducts(string search)
		{
			List<Product> products = GetProducts();
			List<Product> filtered = new List<Product>();
			search = search.ToLower();

			foreach(Product product in products)
			{
				if (product.Name.ToLower().Contains(search) || product.Description.ToLower().Contains(search))
				{
					filtered.Add(product);
				}
			}
			return filtered;
		}

		public static bool IsValidId(string id)
		{
			if (String.IsNullOrEmpty(id)) { return false; }

			int count;
			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT COUNT(*) FROM ProductList WHERE ProductID = @ProductId";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@ProductId", id);

				count = (int) cmd.ExecuteScalar();
			}
			return (count > 0);
		}

		public static Product? GetProductById(string id)
		{
			// Return null if invalid Id
			if (!IsValidId(id)) { return null; }

			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT ProductID, ProductName, Price, Description FROM ProductList
							   WHERE ProductID = @ProductId";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@ProductId", id);
				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Product product = new Product()
					{
						Id = (string)reader["ProductID"],
						Name = (string)reader["ProductName"],
						Description = (string)reader["Description"],
						Price = (decimal)reader["Price"],
						ImageUrl = $"/img/{(string)reader["ProductID"]}.png"
					};
					return product;
				}
			}
			return null;
		}
	}
}
