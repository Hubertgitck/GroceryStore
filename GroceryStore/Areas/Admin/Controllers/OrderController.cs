using Stripe;
using Stripe.Checkout;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
    private readonly StripeSessionProvider _stripeSession;

    [BindProperty]
	public OrderViewModel? OrderViewModel { get; set; }

	public OrderController(IUnitOfWork unitOfWork, StripeSessionProvider stripeSessionProvider)
	{
		_unitOfWork = unitOfWork;
        _stripeSession = stripeSessionProvider;
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

		Session session = _stripeSession.GetStripeSession(orderHeader.SessionId);

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
	public IActionResult UpdateOrderDetail()
	{
		var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
		orderHEaderFromDb.Name = OrderViewModel.OrderHeader.Name;
		orderHEaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
		orderHEaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
		orderHEaderFromDb.City = OrderViewModel.OrderHeader.City;
		orderHEaderFromDb.State = OrderViewModel.OrderHeader.State;
		orderHEaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;
		if (OrderViewModel.OrderHeader.Carrier != null)
		{
			orderHEaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
		}
		if (OrderViewModel.OrderHeader.TrackingNumber != null)
		{
			orderHEaderFromDb.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
		}
		_unitOfWork.OrderHeader.Update(orderHEaderFromDb);
		_unitOfWork.Save();
		TempData["Success"] = "Order Details Updated Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = orderHEaderFromDb.Id });
	}		
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult StartProcessing()
	{
		var id = OrderViewModel.OrderHeader.Id;

		_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusInProcess);
		_unitOfWork.Save();
		TempData["Success"] = "Order Details Updated Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = id });
	}		
	
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult ShipOrder()
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
		orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
		orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
		orderHeader.OrderStatus = SD.StatusShipped;
		orderHeader.ShippingDate = DateTime.Now;

		_unitOfWork.OrderHeader.Update(orderHeader);
		_unitOfWork.Save();
		TempData["Success"] = "Order Shipped Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
	}		

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public IActionResult CancelOrder()
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
			u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);

		if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
		{
			var options = new RefundCreateOptions
			{
				Reason = RefundReasons.RequestedByCustomer,
				PaymentIntent = orderHeader.PaymentIntendId
			};

			var service = new RefundService();
			Refund refund = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
		}
		else
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
		}
		_unitOfWork.Save();

		TempData["Success"] = "Order Cancelled Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
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
