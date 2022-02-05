using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
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
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        // GET: UserController
        public ActionResult Login()
        {
            return View("../Admin/User/Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login( [Bind("Email,Password")] UserModel model)
        { 
            //admin@gmail.com 
            //Med@2000
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
                            return View("../Admin/User/Login");
                        }
                    }
                }
                ModelState.AddModelError("exist", "The User Not found");
                return RedirectToAction(nameof(Login), model);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Signup()
        {
            return View("../Admin/User/Signup");
        }

        

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup([Bind("Email,Name,Password")] UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser
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

        // POST: UserController/Delete/5
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

        public async Task<ActionResult> CreateRole()
        {
            string[] roleNames = new string[] { "Admin", "Manager", "User" };
            List<string> result = new List<string>();
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
