using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.Controllers.User;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();

            });
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<Data.Tables.User, IdentityRole>(
                options => { 
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 3; 
                }
                )
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            services.AddScoped<HttpContextAccessor>();

            //conjection models 
            services.AddScoped<ProductModel>(); 
            services.AddScoped<OrderModel>();
            services.AddScoped<CartController>(); 

            //services.AddIdentity<IdentityUser, IdentityRole>()
            //  .AddEntityFrameworkStores<ApplicationDbContext>();  

            
            //change redirect routes when login in 
            services.ConfigureApplicationCookie(options => {
                options.LoginPath =  "/home/Index";
                options.AccessDeniedPath = "/home/Index"; 
                //options.ClaimsIssuer.
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public  void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapRazorPages();
                endpoints.MapAreaControllerRoute( 
                    name: "default",
                    areaName:"User",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute( 
                    name: "AdminArea",
                    areaName:"Admin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
