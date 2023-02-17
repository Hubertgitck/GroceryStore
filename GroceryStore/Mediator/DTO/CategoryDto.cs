using System.ComponentModel.DataAnnotations;

namespace ApplicationWeb.Mediator.DTO;

public class CategoryDto
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    [Display(Name = "Display Order")]
    [Range(1, 100)]
    public int DisplayOrder { get; set; }
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;

}
