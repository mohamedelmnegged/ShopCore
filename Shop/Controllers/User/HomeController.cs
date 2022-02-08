using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Data.Tables;
using Shop.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shop.Controllers.User
{
    [Area("User")]
    //[AllowAnonymous]
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
            //var header = HttpContext.Request.Headers["Referer"].ToString();
            //Regex reg = new Regex(@"\(admin\)\|\(dashboard\)");

            //return Json(HttpContext.Request.Headers);
            //return Json(header.Contains("dashboard") ); 
            //if (header.Contains("admin") || header.Contains("dashboard"))
            //{
            //    return View("../Admin/User/Login");
            //}
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
        public async Task<ActionResult> Login([Bind("Email,Password")]UserLogin model)
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
                        ModelState.AddModelError("Password", "Password is not Correct");
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Email is not Exsit");
                    }
                }
                return View("../User/Login", model);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        
        [HttpGet]
        public ActionResult Signup()
        {
            return View("../User/Signup");
        }
        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup([Bind("Name,Email,Address,Password,ConfirmPassword,Phone")]UserSignup model)
        {
            try
            {
                if (ModelState.IsValid) { 
                    if(model.Password == null)
                    {
                        ModelState.AddModelError("Password", "The Password is Required");
                        return View("../User/Signup", model); 
                    }
                    //return Json(model); 
                    var user = new Data.Tables.User 
                    { 
                        UserName = model.Name,
                        Email = model.Email , 
                        PhoneNumber = model.Phone, 
                        Address = model.Address
                    };
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
                    return View("../User/Signup", model);
                }
                return View("../User/Signup", model);
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

        [Authorize(Roles = "User")]
        public async Task<ActionResult> Profile()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
                return View("../User/Profile", user);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public async Task<ActionResult> Edit()
        {
            var find = await userManager.FindByNameAsync(User.Identity.Name);
            if (find != null)
            {
                var user = new UserSignup { 
                    Id = find.Id,
                    Email = find.Email,
                    Name = find.UserName, 
                    Phone = find.PhoneNumber,
                    Address = find.Address
                };
                return View("../User/Edit", user);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Edit([Bind("Id,Name,Email,Password,ConfirmPassword,Address,Phone")] UserSignup model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByIdAsync(model.Id);

                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.PhoneNumber = model.Phone;
                    user.UserName = model.Name;
                   
                    var update = await userManager.UpdateAsync(user);
                    if (update.Succeeded)
                    {
                        if(model.Password != null)
                        {
                            var updatePassword = await userManager.AddPasswordAsync(user, model.Password);
                        }
                        await signInManager.RefreshSignInAsync(user);
                        return RedirectToAction(nameof(Profile));
                    }
                    ViewBag.error = "There are an error when updating User"; 
                }
                return View("../User/Edit", model);
            }
            catch
            {
                return View();
            }
        }
    }
}
