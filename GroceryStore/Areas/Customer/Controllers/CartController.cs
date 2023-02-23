using ApplicationWeb.Mediator.Commands.CartCommands;
using ApplicationWeb.Mediator.Requests.CartRequests;
using Stripe.Checkout;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public async Task<IActionResult> SummaryPOST(ShoppingCartViewDto shoppingCartViewDto)
    {
		var result = await _mediator.Send(new SummaryPost(User, shoppingCartViewDto));
		
		if(string.IsNullOrEmpty(result))
		{
			TempData["error"] = "Your cart is empty. Please, add any product first.";
			return RedirectToAction("Index", "Shop");
		}
		else
		{
			Response.Headers.Add("Location", result);
			return new StatusCodeResult(303);
		}
    }

	/*
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

}*/
}
