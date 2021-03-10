using HotelFood.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
using HotelFood.Core;
using HotelFood.Repositories;

namespace HotelFood
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        
            services.AddLogging();
            services.AddIdentity<HotelUser, HotelRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    // .AddDefaultUI()
                    //.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                    .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                //options.Cookie.HttpOnly = true;
                // options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                //options.LoginPath = "/Account/Login";
                options.LoginPath = "/Home/Index";

                options.LogoutPath = "/Account/LogOut";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                //options.SlidingExpiration = true;
            });
            services.AddTransient<ICompanyUserRepository, CompanyUserRepository>();

           // services.AddTransient<IDayDishesRepository, DayDishesRepository>();
            services.AddTransient<IUserDayDishesRepository, UserDayDishesRepository>();
            services.AddTransient<IGenericModelRepository<Dish>, GenericModelRepository<Dish>>();
            
            services.AddTransient<IComplexRepository, ComplexRepository>();
           
            //services.AddTransient<IUserGroupsRepository, UserGroupsRepository>();
            services.AddTransient<ICompanyUserRepository, CompanyUserRepository>();
            //services.AddTransient<IUserFinRepository, UserFinRepository>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient<IGenericModelRepository<Categories>, GenericModelRepository<Categories>>();
            services.AddTransient<IGenericModelRepository<Categories>, GenericModelRepository<Categories>>();
            services.AddTransient<SharedViewLocalizer>();
            //services.AddTransient<URLHelperContextLess>();
            services.AddScoped<IUserClaimsPrincipalFactory<HotelUser>, CustomClaimsPrincipalFactory>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (t, f) => f.Create(typeof(SharedResources)));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();




            app.UseAuthorization();
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
