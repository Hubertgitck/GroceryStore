namespace ApplicationWeb.Areas.Admin.Controllers;


[Area("Admin")]
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
public class PackagingTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public PackagingTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        IEnumerable<PackagingType> packagingTypeList = _unitOfWork.PackagingType.GetAll();
        foreach(var elem in packagingTypeList)
        {
            if (elem.IsWeightInGrams)
            {
                elem.Weight *= SD.KilogramsToGramsFactor;
            }
        }
        return View(packagingTypeList);
    }

    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PackagingType packagingType)
    {
        if (ModelState.IsValid)
        {
            if (packagingType.IsWeightInGrams)
            {
                packagingType.Weight = packagingType.Weight / SD.KilogramsToGramsFactor;
            }
            _unitOfWork.PackagingType.Add(packagingType);
            _unitOfWork.Save();
            TempData["success"] = "Packaging Type created succesfully";
            return RedirectToAction("Index");
        }
        return View(packagingType);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var packagingTypeFromDb = _unitOfWork.PackagingType.GetFirstOrDefault(c => c.Id == id);

        if (packagingTypeFromDb == null)
        {
            return NotFound();
        }

        if (packagingTypeFromDb.IsWeightInGrams)
        {
            packagingTypeFromDb.Weight *= SD.KilogramsToGramsFactor;
        }

        return View(packagingTypeFromDb);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(PackagingType packagingType)
    {
        if (ModelState.IsValid)
        {
            if (packagingType.IsWeightInGrams)
            {
                packagingType.Weight /= SD.KilogramsToGramsFactor;
            }
            _unitOfWork.PackagingType.Update(packagingType);
            _unitOfWork.Save();
            TempData["success"] = "Packaging Type updated succesfully";
            return RedirectToAction("Index");
        }
        return View(packagingType);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var packagingTypeFromDb = _unitOfWork.PackagingType.GetFirstOrDefault(u => u.Id == id);

        if (packagingTypeFromDb == null)
        {
            return NotFound();
        }

        return View(packagingTypeFromDb);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
        var packagingType = _unitOfWork.PackagingType.GetFirstOrDefault(u => u.Id == id);

        if (packagingType == null)
        {
            return NotFound();
        }

        _unitOfWork.PackagingType.Remove(packagingType);
        _unitOfWork.Save();
        TempData["success"] = "Packaging Type deleted succesfully";
        return RedirectToAction("Index");
    }
}
