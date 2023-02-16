namespace ApplicationWeb.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            if (HttpContext.Session.GetInt32(Constants.SessionCart) != null)
            {
                return View(HttpContext.Session.GetInt32(Constants.SessionCart));
            }
            else
            {
                HttpContext.Session.SetInt32(Constants.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count());
                return View(HttpContext.Session.GetInt32(Constants.SessionCart));
            }
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}
