using Microsoft.Data.SqlClient;
using WebApp_ShoppingCart.Models;

namespace WebApp_ShoppingCart.Data
{
	public class DBUser
	{
		// Returns Dictionary of Users from DB User
		public static Dictionary<string, string> GetUserDict()
		{
			Dictionary<string, string> users = new Dictionary<string, string>();

			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT * FROM [User]";
				SqlCommand cmd = new SqlCommand(sql, conn);
				
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					string username = (string)reader["Username"];
					string passhash = (string)reader["Passhash"];
					users.Add(username, passhash);
				}
			}
			return users;
		}

		// Returns DisplayName given Username
		public static string? GetDisplayName(string username)
		{
			string displayname = null;

			using (SqlConnection conn = new SqlConnection(Data.CONNECTION_STRING))
			{
				conn.Open();
				string sql = @"SELECT Displayname FROM [User] WHERE Username=@Username";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.Parameters.AddWithValue("@Username", username);

				displayname = (string) cmd.ExecuteScalar();
			}
			return displayname;
		}
	}
}
