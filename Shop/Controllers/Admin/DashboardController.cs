using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.Admin
{ 
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ProductModel product;

        public DashboardController(ProductModel product)
        {
            this.product = product;
        }
        public IActionResult Index()
        {
            return View("../Admin/index");
        }
    }
}
