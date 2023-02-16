namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
        return View(categoryList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
            TempDataHelper.SetSuccess(this, "Category created succesfully");
            return RedirectToAction("Index");
        }
        return View(category);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            TempDataHelper.SetSuccess(this, "Category updated succesfully");
            return RedirectToAction("Index");
        }
        return View(category);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();
        TempDataHelper.SetSuccess(this, "Category deleted succesfully");
        return RedirectToAction("Index");
    }
}
