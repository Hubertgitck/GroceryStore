﻿using System.ComponentModel.DataAnnotations;
using Application.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Application.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        [Display(Name = "Packaging Type")]
        public int PackagingTypeId { get; set; }
        [ValidateNever]
        public PackagingType PackagingType { get; set; }

        public string GetProductNameWithPackagingType()
        {
            string? nameWithUnits;
            var weight = PackagingType.Weight;
            var name = PackagingType.Name;

            if (PackagingType.IsWeightInGrams)
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
            return Price / PackagingType.Weight;
        }
    }
}
