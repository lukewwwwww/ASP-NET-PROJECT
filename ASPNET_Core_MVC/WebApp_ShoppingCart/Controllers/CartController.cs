using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp_ShoppingCart.Data;
using WebApp_ShoppingCart.Models;

// Handles Cart & Checkout
namespace WebApp_ShoppingCart.Controllers
{
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			ISession sessionObj = HttpContext.Session;
			string? isAuthenticated = sessionObj.GetString("isAuthenticated");
			string? userId = sessionObj.GetString("userId");
			sessionObj.SetString("fromCheckout", "false");

			if (String.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")				// If not logged in
			{                                                                                   // Save sessionId as userId
				sessionObj.SetString("userId", sessionObj.Id);
				ViewBag.debugInfo = $"isAuthenticated = false, userId = {sessionObj.Id}";
			}
			else                                                                                // Else already logged in
			{                                                                                   // Update nav
				ViewBag.displayname = DBUser.GetDisplayName(userId);
				ViewBag.debugInfo = $"isAuthenticated = true, userId = {userId}";
			}

			ViewBag.cart = DBCart.GetCartItems(userId);
            return View();
		}
		
		[HttpPost]
		public IActionResult UpdateCartItem(string productId, int quantity)
		{
			ISession sessionObj = HttpContext.Session;
			string? isAuthenticated = sessionObj.GetString("isAuthenticated");
			string? userId = sessionObj.GetString("userId");

			DBCart.UpdateQuantity(productId, quantity, userId);
			return Ok();
		}

		[HttpPost]
		public IActionResult RemoveCartItem(string productId)
		{
			ISession sessionObj = HttpContext.Session;
			string? isAuthenticated = sessionObj.GetString("isAuthenticated");
			string? userId = sessionObj.GetString("userId");

			DBCart.RemoveItem(productId, userId);
			ViewBag.cartCount = DBCart.GetNonUniqueCount(userId);
			return Json(new { cartCount = ViewBag.cartCount });
		}

		public IActionResult Checkout()
		{
			ISession sessionObj = HttpContext.Session;
			string? isAuthenticated = sessionObj.GetString("isAuthenticated");
			string? userId = sessionObj.GetString("userId");

			if (String.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")             // If not logged in
			{                                                                                   // Redirect User to Login
				sessionObj.SetString("fromCheckout", "true");
				return RedirectToAction("Login", "Account");
			}
			else                                                                                // Else already logged in
			{																					// Checkout items in Cart
				DBPurchase.CheckoutCart(DBCart.GetCartItems(userId), userId);
				return RedirectToAction("History", "Account");
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
