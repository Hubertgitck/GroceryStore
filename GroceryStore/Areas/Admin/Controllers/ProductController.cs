using Application.Models.ViewModels;
using ApplicationWeb.Mediator.Commands.ProductCommands;
using ApplicationWeb.Mediator.Requests.ProductRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
public class ProductController : Controller
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> Upsert(int? id)
    {
        var result = await _mediator.Send(new GetProductViewById(id));
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductViewDto productViewDto, IFormFile? file)
    {
        if (ModelState.IsValid) 
        {
            var result = await _mediator.Send(new UpsertCommand(productViewDto.ProductDto, file));
            TempData["success"] = $"Product {result} successfully";
            return RedirectToAction("Index");
        }
        return View(productViewDto);
    }

    #region API CALLS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productList = await _mediator.Send(new GetAllProducts());
        return Json(new { data = productList });
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await _mediator.Send(new DeleteCommand(id));
        return Json(result);
    }

    #endregion
}
