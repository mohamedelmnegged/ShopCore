using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.ModelViews
{
    public class UserSignup
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Password is Required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Two Password Should be the same")]
        [Display(Name="Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "the Phone is required")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
