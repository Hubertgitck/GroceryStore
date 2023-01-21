using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PriceThresholds
    {
        [Key]
        public int Id { get; set; }
        public string Threshold { get; set; }
        public double Price { get; set; }
    }
}
