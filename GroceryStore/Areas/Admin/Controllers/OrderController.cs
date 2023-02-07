using Stripe;
using Stripe.Checkout;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
    private readonly StripeServiceProvider _stripeServices;


    [BindProperty]
	public OrderViewModel? OrderViewModel { get; set; }

	public OrderController(IUnitOfWork unitOfWork, StripeServiceProvider stripeServices)
	{
		_unitOfWork = unitOfWork;
        _stripeServices = stripeServices;
    }

	public IActionResult Index()
	{
		return View();
	}		
	public IActionResult Details(int orderId)
	{  
		OrderViewModel = new OrderViewModel()
		{
			OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
			OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product", thenIncludeProperty: "PackagingType")
        };

		return View(OrderViewModel);
	}

	public IActionResult PaymentConfirmation(int orderHeaderId)
	{
		OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId);

		Session session = _stripeServices.GetStripeSession(orderHeader.SessionId);

		if (session.PaymentStatus.ToLower() == "paid")
		{
			_unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, orderHeader.SessionId, session.PaymentIntentId);
			_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
			_unitOfWork.Save();
		}

		return View(orderHeaderId);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult UpdateOrderDetail(OrderViewModel orderViewModel)
	{
		var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == orderViewModel.OrderHeader.Id, tracked: false);
        orderHeaderFromDb.Name = orderViewModel.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = orderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = orderViewModel.OrderHeader.City;
        orderHeaderFromDb.State = orderViewModel.OrderHeader.State;
        orderHeaderFromDb.PostalCode = orderViewModel.OrderHeader.PostalCode;
		if (orderViewModel.OrderHeader.Carrier != null)
		{
            orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
		}
		if (orderViewModel.OrderHeader.TrackingNumber != null)
		{
            orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
		}
		_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
		_unitOfWork.Save();
		TempData["Success"] = "Order Details Updated Successfully";
		return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
	}		

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult StartProcessing(OrderViewModel orderViewModel)
	{
		var id = orderViewModel.OrderHeader.Id;

		_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusInProcess);
		_unitOfWork.Save();
		TempData["Success"] = "Order processing started";
		return RedirectToAction("Details", "Order", new { orderId = id });
	}		
	
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult ShipOrder(OrderViewModel orderViewModel)
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == orderViewModel.OrderHeader.Id, tracked: false);
		orderHeader.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
		orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
		orderHeader.OrderStatus = SD.StatusShipped;
		orderHeader.ShippingDate = DateTime.Now;

		_unitOfWork.OrderHeader.Update(orderHeader);
		_unitOfWork.Save();
		TempData["Success"] = "Order Shipped Successfully";
		return RedirectToAction("Details", "Order", new { orderId = orderViewModel.OrderHeader.Id });
	}		

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult CancelOrder(OrderViewModel orderViewModel)
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == orderViewModel.OrderHeader.Id, tracked: false);

		if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
		{
			_stripeServices.GetRefundService(orderHeader.PaymentIntendId!);
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
		}
		else
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
		}
		_unitOfWork.Save();

		TempData["Success"] = "Order Cancelled Successfully";
		return RedirectToAction("Details", "Order", new { orderId = orderViewModel.OrderHeader.Id });
	}

	#region API CALLS
	[HttpGet]
	public IActionResult GetAll(string status)
	{
		IEnumerable<OrderHeader> orderHeaders;

		if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
		{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
        }
		else
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(
				u => u.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");
        }

		switch (status)
		{
			case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
            case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;             
			case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
            default:
                    break;
        }

		return Json(new { data = orderHeaders });
	}
	#endregion
}
