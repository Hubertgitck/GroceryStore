﻿using System.Diagnostics;
using System.Security.Claims;
using ApplicationWeb.Mediator.Requests.ShopRequests;

namespace ApplicationWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class ShopController : Controller
{
    private readonly IMediator _mediator;

    public ShopController(IMediator mediator)
	{
        _mediator = mediator;
    }

	public async Task<IActionResult> Index(string category)
	{
		var result = await _mediator.Send(new GetShopIndexView(category));
		return View(result);
    } 
	
	public IActionResult Details(int productId)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == productId);

        Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,PackagingType");

        if (cartFromDb != null)
		{
			cartFromDb.Product = product;
            return View(cartFromDb);
		}
		else
		{
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = product
            };
            return View(cartObj);
        }	
	}
    /*
    [HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public IActionResult Details(ShoppingCart shoppingCart)
	{
        var claim = GetUserClaim();
        shoppingCart.ApplicationUserId = claim.Value;

		ShoppingCart cartFromDb = GetCartByUserClaimAndProductId(claim, shoppingCart.ProductId);

		if (cartFromDb == null)
		{
			_unitOfWork.ShoppingCart.Add(shoppingCart);
			_unitOfWork.Save();
			TempData["success"] = "Product added to cart succesfully";
            HttpContext.Session.SetInt32(Constants.SessionCart,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count());
		}
		else
		{
			cartFromDb.Count = shoppingCart.Count;
            TempData["success"] = "Shopping Cart updated succesfully";
            _unitOfWork.ShoppingCart.Update(cartFromDb);
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



        private ShoppingCart GetCartByUserClaimAndProductId(Claim claim, int productId)
        {

        }*/
}