using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers.User
{ 
    [Area("User")]
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly UserManager<Data.Tables.User> userManager;
        private readonly ApplicationDbContext context;

        public CartController(UserManager<Data.Tables.User> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public async Task<IActionResult> Add(int Id)
        {
            if(Id != null)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name); 
                if(user != null)
                {
                    var checkOrder = context.Orders.Where(a => a.ProductId == Id && a.UserId == user.Id);
                   
                    if (checkOrder.Count() > 0)
                    {
                        checkOrder.FirstOrDefault().Amount += 1;
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        // make an order 
                        var order = new Order
                        {
                            UserId = user.Id,
                            ProductId = Id,
                            Amount = 1,
                            Status = Enums.Status.worked,
                            Purchase = Enums.Purchase.inAction
                        };
                        var add =  await context.AddAsync(order);
                        await context.SaveChangesAsync();
                        // 
                        var orderId = context.Orders.Where(a => a.ProductId == Id && a.UserId == user.Id).FirstOrDefault(); 
                        var cart = new Cart { OrderId = orderId.Id };
                        await context.Carts.AddAsync(cart);
                        await context.SaveChangesAsync();

                    }

                    return Json("Done"); 
                }
            }
            return View(); 
        }
    }
}
