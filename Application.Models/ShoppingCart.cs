﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Application.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }

    [Range(1, 10000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }
    [NotMapped]
    public double Price { get; set; }
}
