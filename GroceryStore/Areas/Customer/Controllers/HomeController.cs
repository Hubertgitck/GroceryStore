namespace ApplicationWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    public HomeController()
    {

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromQuery] ErrorViewDto errorViewDto)
    {
        return View(errorViewDto);
    }
}