using ApplicationWeb.Mediator.Commands.CategoryCommands;
using ApplicationWeb.Mediator.Requests.CategoryRequests;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
public class CategoryController : Controller
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetAllCategories());
        return View(result);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto category)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(new AddCategory(category));
            TempDataHelper.SetSuccess(this, "Category created succesfully");
            return RedirectToAction("Index");
        }
        return View(category);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        try
        {
            var result = await _mediator.Send(new GetCategoryById(id));
            return View(result);
        }
        catch (Exception)
        {
            return NotFound();
        } 
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryDto category)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(new EditCategory(category));
            TempDataHelper.SetSuccess(this, "Category updated succesfully");
            return RedirectToAction("Index");
        }
        return View(category);
    }
       
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            var result = await _mediator.Send(new GetCategoryById(id));
            return View(result);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        try
        {
            await _mediator.Send(new DeleteCategory(id));
            TempDataHelper.SetSuccess(this, "Category deleted succesfully");
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}
