using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp_ShoppingCart.Data;
using WebApp_ShoppingCart.Models;

// Handles Login, Logout, and Saved Account Data (Purchase History)
namespace WebApp_ShoppingCart.Controllers
{
	public class AccountController : Controller
	{
		/*	For testing purposes, use the following username/password/passhash:
		 *	admin, Password123, b2e98ad6f6eb8508dd6a14cfa704bad7f05f6fb1
		 *	john, password1, e38ad214943daad1d64c102faec29de4afe9da3d
		 */

		[HttpGet]
		public IActionResult Login()
		{
			string? isAuthenticated = HttpContext.Session.GetString("isAuthenticated");

			if (String.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")     // If not logged in
			{                                                                           // Display common view
				ViewBag.debugInfo = $"isAuthenticated = false, userId = none";
				return View();
			}
			else                                                                        // Else already logged in
			{                                                                           // Redirect to Gallery
				return RedirectToAction("Gallery", "Shop");
			}			
		}

		[HttpPost]
		public IActionResult Login(string username, string passhash)
		{
			Dictionary<string, string> users = DBUser.GetUserDict();
			ISession sessionObj = HttpContext.Session;
			string? fromCheckout = sessionObj.GetString("fromCheckout");

			if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(passhash))		// Invalid values
			{
				ViewBag.debugInfo = $"isAuthenticated = false, userId = none";
				return View();
			}
			else if (users.ContainsKey(username.ToLower()))								// Login was attempted
			{
				if (users[username.ToLower()] == passhash)								// Login success
				{																		// Update session details
					sessionObj.SetString("isAuthenticated", "true");
					sessionObj.SetString("userId", username);

					if (String.IsNullOrEmpty(fromCheckout) || fromCheckout != "true")	// If not logging in after Checkout button
					{                                                                   // Redirect to Gallery
						return RedirectToAction("Gallery", "Shop");
					}
					else																// User came directly from Checkout
					{                                                                   // Reassign cart items to new user, Checkout again
						DBCart.UpdateCartUser(sessionObj.Id, username);
						return RedirectToAction("Checkout", "Cart");
					}
				}
			}

			// Login attempt fails
			ViewBag.loginError = "The username or password entered is incorrect. Please try again.";
			ViewBag.debugInfo = $"isAuthenticated = false, userId = none";
			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}

		public IActionResult History()
		{
			ISession sessionObj = HttpContext.Session;
			string? isAuthenticated = sessionObj.GetString("isAuthenticated");
			string? userId = sessionObj.GetString("userId");
			sessionObj.SetString("fromCheckout", "false");

			if (String.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")     // If not logged in
			{                                                                           // Redirect to Login page
				return RedirectToAction("Login", "Account");
			}
			else                                                                        // Else already logged in
			{                                                                           // Display user-specific view
				List<Purchase> purchases = DBPurchase.GetPurchaseHistory(userId);
				ViewBag.purchases = purchases;
				ViewBag.displayname = DBUser.GetDisplayName(userId);
				ViewBag.debugInfo = $"isAuthenticated = true, userId = {userId}";
				return View();
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
