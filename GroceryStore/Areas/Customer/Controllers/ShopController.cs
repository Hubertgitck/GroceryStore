using ApplicationWeb.Mediator.Commands.ShopCommands;
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

	[Authorize]
	public async Task<IActionResult> Details(int productId)
	{
		var result = await _mediator.Send(new GetProductDetails(User, productId));
		return View(result);
	}
    
    [HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public async Task<IActionResult> Details(ShoppingCartDto shoppingCartDto)
	{
		var result = await _mediator.Send(new DetailsPost(shoppingCartDto, User));
		TempData["success"] = result;
		return RedirectToAction(nameof(Index));
	}
}