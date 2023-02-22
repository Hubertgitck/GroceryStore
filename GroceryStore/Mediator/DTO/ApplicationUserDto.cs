using System.ComponentModel.DataAnnotations;

namespace ApplicationWeb.Mediator.DTO
{
    public class ApplicationUserDto
	{
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
