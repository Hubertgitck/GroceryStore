using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ApplicationWeb.Mediator.DTO
{
	public class ShoppingCartDto
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		[ValidateNever]
		public ProductDto ProductDto { get; set; }

		[Range(1, 10000, ErrorMessage = "Please enter a value between 1 and 1000")]
		public int Count { get; set; }
		public string ApplicationUserId { get; set; }
		[ValidateNever]
		public ApplicationUserDto ApplicationUserDto { get; set; }
		public double Price { get; set; }
	}
}
