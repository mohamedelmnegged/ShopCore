using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Tables;
using Shop.ModelViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.Controllers.User
{ 
    [Area("User")]
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly UserManager<Data.Tables.User> userManager;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<Data.Tables.User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartController(UserManager<Data.Tables.User> userManager, ApplicationDbContext context, 
            SignInManager<Data.Tables.User> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.context = context;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
        }   
        public IEnumerable<CartView> GetCartViews()
        { 
            
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var model = (from order in context.Orders.Where(a => a.UserId == userId)
                         join cart in context.Carts on order.Id equals cart.OrderId
                         join product in context.Products on order.ProductId equals product.Id
                         select new CartView
                         {
                             Image = product.Image,
                             Amount = order.Amount,
                             Name = product.Name,
                             Description = product.Description,
                             Price = product.Price,
                             OrderId = order.Id
                         });
            return model; 
        }
        public async Task<IActionResult> Index()
        {
            var model = GetCartViews(); 
            return View("../User/Cart", model);
        }

        [HttpPost] 
        public async Task<IActionResult> UpdateCounter(List<List<int>> counter)
        { 
            //update amounts of every product in orders 
            foreach (List<int> i in counter)
            {
                var order = context.Orders.Where(a => a.Id == i[0]).FirstOrDefault();
                var product = context.Products.Where(a => a.Id == order.ProductId).FirstOrDefault();
                if (i[1] <= product.Quantity)
                {
                    order.Amount = i[1];
                    product.Quantity -= i[1]; 
                    context.Update(order);
                }
            }
            await context.SaveChangesAsync();
            return Json(new { redirectToUrl = Url.Action("ProceedCheckout", "Cart") });
        } 

        public IActionResult ProceedCheckout()
        {
            return View("../User/Checkout");
        } 

        [HttpPost]
        public async Task<IActionResult> ProceedCheckout([Bind("Email,Name,NameOnCard,Phone,Card,Address,City,Provance,Postal")] Checkout model)
        {
            return Json(model);
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
                    var product = context.Products.Where(a => a.Id == Id).FirstOrDefault(); 
                    var count = context.Orders.Where(a => a.User.UserName == User.Identity.Name)
                        .Join(context.Carts, order => order.Id, cart => cart.OrderId,
                                    (order, cart) => new
                                    {
                                        order = order.Id
                                    }
                                    ).Count();

                    if (checkOrder.Count() > 0)
                    {
                        if (product.Quantity <= checkOrder.FirstOrDefault().Amount + 1)
                        {
                            checkOrder.FirstOrDefault().Amount += 1;
                        }
                        await context.SaveChangesAsync();
                        
                        return Json(new { Count = count, msg = "Already exsit in Cart and increased Amount" });
                    }
                    else
                    {
                        // make an order 
                        if (product.Quantity > 0)
                        {
                            var order = new Order
                            {
                                UserId = user.Id,
                                ProductId = Id,
                                Amount = 1,
                                Status = Enums.Status.worked,
                                Purchase = Enums.Purchase.inAction
                            };
                            var add = await context.AddAsync(order);
                            await context.SaveChangesAsync();
                            // 
                            var orderId = context.Orders.Where(a => a.ProductId == Id && a.UserId == user.Id).FirstOrDefault();
                            var cart = new Cart { OrderId = orderId.Id };
                            await context.Carts.AddAsync(cart);
                            await context.SaveChangesAsync(); 

                            return Json(new { Count = count + 1, msg = "Added To Cart Successfuly" });
                        }
                        

                        return Json(new {Count = count , msg = "There is no Engough Quantity in the Stock" });
                    }
                }
            }
            return Json("The Id not exsit"); 
        } 
        [HttpPost] 
        public async Task<IActionResult> Remove([Bind("id")] int Id)
        {
            var find = context.Orders.Where(a => a.Id == Id).FirstOrDefault();
            var remove = context.Orders.Remove(find);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }
        public async Task<IActionResult> ReturnToBuy([Bind("id")] int id)
        {
            //add to cart 
            if (id != null)
            {
                var find = context.Carts.Where(a => a.OrderId == id).FirstOrDefault();
                if (find == null)
                {
                    var add = new Cart { OrderId = id };
                    await context.Carts.AddAsync(add);
                    await context.SaveChangesAsync();
                }
            }

            return RedirectToAction("index");
        }
        public async Task<IActionResult> SaveForLater([Bind("id")] int id)
        {
            //remove order from carts 
            if(id != null)
            {
                var find = context.Carts.Where(a => a.OrderId == id).FirstOrDefault(); 
                if(find != null)
                {
                    var remove = context.Carts.Remove(find);
                    await context.SaveChangesAsync();
                }
            }

            return RedirectToAction("index");
        } 
        public  IEnumerable<CartView> SaveLater()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var model =  context.Orders.Where(a => a.UserId == userId)
                .Where(a => !context.Carts.Select(s => s.OrderId).Contains(a.Id))
                .Join(context.Products, order => order.ProductId, product => product.Id,
                (order, product) => new CartView
                {
                    Name = product.Name,
                    Amount = order.Amount,
                    Description = product.Description,
                    Image = product.Image,
                    OrderId = order.Id,
                    Price = product.Price
                }).AsEnumerable();
            return  model; 
        } 

    }
}
