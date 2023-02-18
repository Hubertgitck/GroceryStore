using System.ComponentModel.DataAnnotations;

namespace ApplicationWeb.Mediator.DTO;

public class PackagingTypeDto
{
    public int Id { get; set; }
    [Display(Name = "Packaging")]
    [Required]
    [MaxLength(25)]
    public string Name { get; set; }
    public double Weight { get; set; }
    [Display(Name = "Unit type")]
    public bool IsWeightInGrams { get; set; }
}
