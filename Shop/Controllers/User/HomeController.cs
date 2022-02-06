using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.User
{
    [Area("User")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<Data.Tables.User> userManager;
        private readonly SignInManager<Data.Tables.User> signInManager;

        public HomeController(ApplicationDbContext context,
            UserManager<Data.Tables.User> userManager, SignInManager<Data.Tables.User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        // GET: HomeController
        public async Task<ActionResult> Index()
        {
            return View("../User/index", await context.Products.ToListAsync());
        }


        // GET: HomeController/Create
        public ActionResult Login()
        {
            return View("../User/Login");
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind("Email,Password")]UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var find = await userManager.FindByEmailAsync(model.Email); 
                    if(find != null)
                    {
                        var check = await userManager.CheckPasswordAsync(find, model.Password);
                        if (check)
                        {
                            await signInManager.SignInAsync(find, isPersistent:false);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                return View("../User/Login", model);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Signup()
        {
            return View("../User/Signup");
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup([Bind("Name,Email,Password")]UserModel model)
        {
            try
            {
                if (ModelState.IsValid) {
                    var user = new Data.Tables.User {UserName = model.Name, Email = model.Email };
                    var add = await userManager.CreateAsync(user, model.Password);
                    if (add.Succeeded)
                    {
                        var role = await userManager.AddToRoleAsync(user, "User");
                        if (role.Succeeded)
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    return RedirectToAction(nameof(Signup));
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index)); 
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
