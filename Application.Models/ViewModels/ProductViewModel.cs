using System.Collections;
using Application.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? PackagingTypeList { get; set; }

        public string GetProductNameWithPackagingType()
        {
            string? nameWithUnits;
            var weight = Product.PackagingType.Weight;
            var name = Product.PackagingType.Name;
            

            if (Product.PackagingType.IsWeightInGrams)
            {
                weight *= SD.KilogramsToGramsFactor;
                nameWithUnits = weight.ToString() + "g " + name;
            }
            else
            {
                nameWithUnits = weight.ToString() + "kg " + name;
            }

            return nameWithUnits;
        }

        public double GetPriceFor1kg()
        {
            return Product.Price / Product.PackagingType.Weight;
        }

    }
}
