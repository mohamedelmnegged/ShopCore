using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Tables;
using Shop.ModelViews;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
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
        public async Task<IActionResult> Index()
        {
          var model =   (from order in context.Orders.Where(a => a.User.UserName == User.Identity.Name)
             join cart in context.Carts on order.Id equals cart.OrderId
             join product in context.Products on order.ProductId equals product.Id
             select new CartView
             {
                 Image = product.Image,
                 Amount = order.Amount,
                 Name = product.Name,
                 Description = product.Description,
                 Price = product.Price 
             });
            return View("../User/Cart", model);
        }
        public record dd();
        [HttpPost] 
        public async Task<IActionResult> UpdateCounter(string[] counter)
        {
          //  var data = HttpContext.Response.Headers; 
            return Json(counter);
        }
        [HttpPost]
        public async Task<IActionResult> Add(int Id)
        {
            if(Id != null)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name); 
                if(user != null)
                {
                    var checkOrder = context.Orders.Where(a => a.ProductId == Id && a.UserId == user.Id);
                    var count = context.Orders.Where(a => a.User.UserName == User.Identity.Name)
                        .Join(context.Carts, order => order.Id, cart => cart.OrderId,
                                    (order, cart) => new
                                    {
                                        order = order.Id
                                    }
                                    ).Count();

                    if (checkOrder.Count() > 0)
                    {
                        checkOrder.FirstOrDefault().Amount += 1;
                        await context.SaveChangesAsync();
                        
                        return Json(new { Count = count, msg = "Already exsit in Cart and increased Amount" });
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

                        return Json(new {Count = count + 1, msg = "Added To Cart Successfuly" });
                    }
                }
            }
            return Json("The Id not exsit"); 
        }
    }
}
