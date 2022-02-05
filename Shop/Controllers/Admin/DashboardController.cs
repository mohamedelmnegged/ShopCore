using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.Admin
{ 
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ProductModel product;
        private readonly RoleManager<IdentityRole> roleManager;

        public DashboardController(ProductModel product, RoleManager<IdentityRole> roleManager)
        {
            this.product = product;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View("../Admin/index");
        }

       
    }
}
