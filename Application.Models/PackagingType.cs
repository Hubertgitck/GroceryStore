using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Application.Models
{
    public class PackagingType
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
}
