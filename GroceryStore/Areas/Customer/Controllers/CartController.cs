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

	public async Task<IActionResult> OrderConfirmation(int id)
	{
		await _mediator.Send(new OrderConfirmation(id));

		HttpContext.Session.Clear();
		return View(id);
	}

	
	public async Task<IActionResult> IncrementCount(int cartId)
	{
		await _mediator.Send(new IncrementCount(cartId));

		return RedirectToAction(nameof(Index));
	}
	
	public async Task<IActionResult> DecrementCount(int cartId)
	{
		await _mediator.Send(new DecrementCount(cartId));

		return RedirectToAction(nameof(Index));
	}
	
	public async Task<IActionResult> Remove(int cartId)
	{
		await _mediator.Send(new RemoveCartById(cartId));

		return RedirectToAction(nameof(Index));
	}	
}
