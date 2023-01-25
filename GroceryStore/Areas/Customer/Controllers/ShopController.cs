using System.Diagnostics;
using System.Security.Claims;
using Application.DataAccess.Repositories.IRepository;
using Application.Models;
using Application.Models.ViewModels;
using Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public ShopController(ILogger<ShopController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
            List<ProductViewModel> productsViewModelsList = new List<ProductViewModel>();
            var productsList = _unitOfWork.Product.GetAll(includeProperties: "Category,PackagingType");
            
			foreach(var product in productsList)
			{
				ProductViewModel productViewModel = new ProductViewModel
				{
					Product = product
				};
				productsViewModelsList.Add(productViewModel);
            }
				
			return View(productsViewModelsList);
		}
		public IActionResult Details(int productId)
		{
			ShoppingCart cartObj = new()
			{
				Count = 1,
				ProductId = productId,
				Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,PackagingType")
			};
			return View(cartObj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCart.ApplicationUserId = claim.Value;

			ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
				u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

			if (cartFromDb == null)
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);

				_unitOfWork.Save();
				HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count());

			}
			else
			{
				_unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
				_unitOfWork.Save();
			}


			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}