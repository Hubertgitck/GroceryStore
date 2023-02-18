using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
using ApplicationWeb.Mediator.Requests.OrderDetailRequests;
using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IMediator _mediator;

    [BindProperty]
	public OrderDto? OrderViewModel { get; set; }

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Index()
	{
		return View();
	}		
	public async Task<IActionResult> Details(int orderId)
	{  
		var orderDto = new OrderDto()
		{
			OrderHeaderDto = await _mediator.Send(new GetOrderHeaderById(orderId)),
			OrderDetailDtos = await _mediator.Send(new GetAllOrderDetailsById(orderId))
        };

		return View(orderDto);
	}

	public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
	{
		await _mediator.Send(new PaymentConfirmation(orderHeaderId));

		return View(orderHeaderId);
	}
    
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
	public async Task<IActionResult> UpdateOrderDetail(OrderHeaderDto orderHeaderDto)
	{
		var result = await _mediator.Send(new UpdateOrderHeader(orderHeaderDto));
		TempDataHelper.SetSuccess(this, "Order Details Updated Successfully");

		return RedirectToAction("Details", "Order", new { orderId = result });
	}
    
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
	public async Task<IActionResult> StartProcessing(OrderHeader orderHeaderDto)
	{
		await _mediator.Send(new StartProcessing(orderHeaderDto.Id));

        TempDataHelper.SetSuccess(this, "Order processing started");
        return RedirectToAction("Details", "Order", new { orderId = orderHeaderDto.Id });
	}

    
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
	public async Task<IActionResult> ShipOrder(OrderHeaderDto orderHeaderDto)
	{
		await _mediator.Send(new ShipOrder(orderHeaderDto));

        TempDataHelper.SetSuccess(this, "Order Shipped Successfully");
        return RedirectToAction("Details", "Order", new { orderId = orderHeaderDto.Id });
	}

    
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
	public async Task<IActionResult> CancelOrder(OrderHeaderDto orderHeaderDto)
	{
		var orderId = orderHeaderDto.Id;
		await _mediator.Send(new CancelOrder(orderId));

		TempDataHelper.SetSuccess(this, "Order Cancelled Successfully");
		return RedirectToAction("Details", "Order", new { orderId });
	}

    #region API CALLS
    [HttpGet]
	public async Task<IActionResult> GetAll(string status)
	{
		var result = await _mediator.Send(new GetAllOrderHeaders(User, status));

		return Json(new { data = result });
	}
	#endregion
}
