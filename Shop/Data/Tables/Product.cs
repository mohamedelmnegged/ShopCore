using Shop.Enums;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
#nullable disable

namespace Shop.Data.Tables
{
    public partial class Product
    {
        public int Id { get; set; } 
        [Required]
        [Display(Name ="Product Name")]
        public string Name { get; set; } 
        public string Description { get; set; }
        [Required]
        public float Price { get; set; }
        public string Image { get; set; }
        public Status Status { get; set; }
        [Required]
        public int Quantity { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
