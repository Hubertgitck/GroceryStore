using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using ApplicationWeb.Mediator.DTO;
using ApplicationWeb.Mediator.Requests.CategoryRequests;
using ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
public class PackagingTypeController : Controller
{
    private readonly IMediator _mediator;

    public PackagingTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetAllPackagingTypes());

        return View(result);
    }

   public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PackagingTypeDto packagingTypeDto)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(new AddPackagingType(packagingTypeDto));
            TempData["success"] = "Packaging Type created succesfully";
            return RedirectToAction("Index");
        }
        return View(packagingTypeDto);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var result = await _mediator.Send(new GetPackagingTypeById(id));
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PackagingTypeDto packagingTypeDto)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(new EditPackagingType(packagingTypeDto));
            TempData["success"] = "Packaging Type updated succesfully";
            return RedirectToAction("Index");
        }
        return View(packagingTypeDto);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var result = await _mediator.Send(new GetPackagingTypeById(id));

        return View(result);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePOST(int? id)
    {
        await _mediator.Send(new DeletePackagingType(id));
        TempData["success"] = "Packaging Type deleted succesfully";
        return RedirectToAction("Index");
    }
}
