﻿using ApplicationWeb.Mediator.Requests.CartRequests;

namespace ApplicationWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
	private readonly IMediator _mediator;

	public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetCartIndexView(User));
		return View(result);
    }
    
    public async Task<IActionResult> Summary()
    {
		var result = await _mediator.Send(new GetSummaryView(User));
		return View(result);
	}
	/*
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public IActionResult SummaryPOST()
    {
        var claim = GetUserClaim();

        ShoppingCartViewModel.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
            includeProperties: "Product");

        if (ShoppingCartViewModel.CartList.Any())
        {
            return ProceedToPayment(claim);
        }
        else
        {
            TempData["error"] = "Your cart is empty. Please, add any product first.";
            return RedirectToAction("Index", "Shop");
        }
    }

	
    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

        var service = new SessionService();
        Session session = service.Get(orderHeader.SessionId);
        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateStatus(id, Constants.StatusApproved, Constants.PaymentStatusApproved);
            _unitOfWork.Save();
        }

        //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Grocery Store","<p>New Order Created</p>");

        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
            .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        HttpContext.Session.Clear();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();

        return View(id);
    }

    public IActionResult Plus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Minus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        if (cart.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cart);
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count() - 1;
            HttpContext.Session.SetInt32(Constants.SessionCart, count);
        }
        else
        {
            _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Remove(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Save();
        var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
        HttpContext.Session.SetInt32(Constants.SessionCart, count);
        return RedirectToAction(nameof(Index));
    }

    private IActionResult ProceedToPayment(Claim claim)
    {
        ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
        ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

        foreach (var cart in ShoppingCartViewModel.CartList)
        {
            cart.Price = cart.Count * cart.Product.Price;
            ShoppingCartViewModel.OrderHeader.OrderTotal += cart.Price;
        }
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

        ShoppingCartViewModel.OrderHeader.PaymentStatus = Constants.PaymentStatusPending;
        ShoppingCartViewModel.OrderHeader.OrderStatus = Constants.StatusPending;

        _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
        _unitOfWork.Save();

        foreach (var cart in ShoppingCartViewModel.CartList)
        {
            OrderDetail order = new()
            {
                ProductId = cart.ProductId,
                OrderId = ShoppingCartViewModel.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count
            };
            _unitOfWork.OrderDetail.Add(order);
            _unitOfWork.Save();
        }
        //Stripe settings

        var domain = ShoppingCartViewModel.PaymentDomain;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
            CancelUrl = domain + $"customer/cart/index",
        };

        foreach (var item in ShoppingCartViewModel.CartList)
        {
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(sessionLineItem);
            }
        }
        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartViewModel.OrderHeader.Id,
            session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }*/
}
