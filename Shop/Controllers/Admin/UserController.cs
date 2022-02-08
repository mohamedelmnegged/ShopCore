using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using Shop.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.Admin
{
    [Area("Admin")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly UserManager<Data.Tables.User> userManager;
        private readonly SignInManager<Data.Tables.User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;

        public UserController(UserManager<Data.Tables.User> userManager, 
            SignInManager<Data.Tables.User> signInManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.context = context;
        } 
        public async Task<ActionResult> Index()
        {
            List<UserView> model = new (); 
            var users = userManager.Users; 
            foreach(var user in users)
            {
                var role = await userManager.GetRolesAsync(user);
                var m = new UserView { Email = user.Email, Name = user.UserName, Role = role.FirstOrDefault() };
                model.Add(m);
            }
            return View("../Admin/User/Index", model);
        }
        // GET: UserController
        public ActionResult Login()
        {
            return View("../Admin/User/Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login( [Bind("Email,Password")] UserLogin model)
        { 
            //admin@gmail.com 
            //Med@2000 
            //mohamed@gmail.com
            //123456
            try
            {
                if (ModelState.IsValid)
                {
                    var find = await userManager.FindByEmailAsync(model.Email);
                    
                    if (find != null)
                    {
                        if (await userManager.CheckPasswordAsync(find, model.Password))
                        {
                            var check = await userManager.IsInRoleAsync(find, "Admin");
                            if (check)
                            {
                                await signInManager.SignInAsync(find, isPersistent: false);
                                return View("../Admin/Index");
                            }

                            ModelState.AddModelError("role", "the user is not an admin");
                            return View("../Admin/User/Login", model);
                        }
                    }
                }
                ModelState.AddModelError("exist", "The User Not found");
                return View("../Admin/User/Login", model);
            }
            catch
            {
                return RedirectToAction(nameof(Login), model);
            }
        }
        public ActionResult Signup()
        {
            return View("../Admin/User/Signup");
        }

        

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup([Bind("Email,Name,Password")] UserSignup model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new Data.Tables.User
                    {
                        UserName = model.Name,
                        Email = model.Email
                    };
                    var add = await userManager.CreateAsync(user, model.Password);
                    if (add.Succeeded)
                    {
                        user = await userManager.FindByEmailAsync(model.Email);
                        var sign = await userManager.AddToRoleAsync(user, "Admin");
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return View("../Admin/Index");
                    }
                }
                return View("../Admin/User/Signup", model);// RedirectToAction(nameof(Signup));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login)); 
        }
        [HttpGet]
        public async Task<ActionResult> Delete(string Name)
        {
            try
            {
                var user = await userManager.FindByNameAsync(Name);
                if (user != null)
                {
                    var model = new UserView { Email = user.Email, Name = user.UserName };
                    return View("../Admin/User/Delete", model);
                }
                return NotFound();
            }
            catch
            {
                return View();  
            }
             
        }
        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind("Name,Email,Role")] UserView model)
        {
            try
            {
                var check = await userManager.FindByNameAsync(model.Name);
                if(check != null)
                {
                   await userManager.DeleteAsync(check);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> CreateRole()
        {
            string[] roleNames = new string[] { "Admin", "Manager", "User" };
            List<string> result = new();
            foreach (string name in roleNames)
            {
                var role = new IdentityRole { Name = name };
                IdentityResult saveRole = await roleManager.CreateAsync(role);
                if (saveRole.Succeeded)
                    result.Add("Succeed");
            }
            return Json(result);

        }
    }
}
