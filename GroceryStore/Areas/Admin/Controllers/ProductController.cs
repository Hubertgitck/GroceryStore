using Application.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApplicationWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleEmployee)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Upsert(int? id, ProductViewModel productViewModel)
    {
        productViewModel.Product = new();
        productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
        productViewModel.PackagingTypeList = _unitOfWork.PackagingType
                .GetAll()
                .OrderBy(u => u.Weight)
                .Select(u => new SelectListItem
                    {
                        Text = u.IsWeightInGrams == true ? u.Name +
                            $" {u.Weight * Constants.KilogramsToGramsFactor}[g]" : u.Name + $" {u.Weight}[kg]",
                        Value = u.Id.ToString(),
                    });

        if (id == null || id == 0)
        {
            return View(productViewModel);
        }
        else
        {
            productViewModel.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productViewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
    {
        if (ModelState.IsValid) 
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if(file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"img\products");
                var extension = Path.GetExtension(file.FileName);

                if(productViewModel.Product.ImageUrl !=null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                productViewModel.Product.ImageUrl = @"\img\products\" + fileName + extension;
            }

            if (productViewModel.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productViewModel.Product);
                TempData["success"] = "Product created successfully";
            }
            else
            {
                _unitOfWork.Product.Update(productViewModel.Product);
                TempData["success"] = "Product updated successfully";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        return View(productViewModel);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,PackagingType");
        return Json(new { data = productList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Eror while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();


        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}
