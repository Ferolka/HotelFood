using System;
using System.Collections.Generic;
using System.Linq;
using HotelFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelFood.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Entity;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HotelFood.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context, IServiceProvider service, IHostEnvironment env)
        {
            context.Database.EnsureCreated();

            var roleManager = service.GetRequiredService<RoleManager<HotelRole>>();
            var userManager = service.GetRequiredService<UserManager<HotelUser>>();
            DateTime dayDate = DateTime.Now;
            //  context.SetCompanyID(1);
            //  var queue = context.DeliveryQueues.Where(dq => dq.UserId == "27fb457f-8b4f-4a66-96ce-5e98ae2f1d91" && dq.DayDate == dayDate.ResetHMS()).ToList();

           
            if (env.EnvironmentName != "LocalProduction")
            {
                CreateAdminRole(context, roleManager, userManager);
                CreateRole(UserExtension.UserRole_CompanyAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_GroupAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_UserAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_KitchenAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_SubGroupAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_ServiceAdmin, context, roleManager);
                CreateRole(UserExtension.UserRole_SubGroupReportAdmin, context, roleManager);

                
                SQLScriptExecutor executor = new SQLScriptExecutor(context, service);
                executor.ExecuteStartScripts();
                
                if (context.Dish.IgnoreQueryFilters().Any())
                {
                    return;
                }
            }
            
            return; //danger
            ClearDatabase(context);
            CreateAdminRole(context, roleManager, userManager);
            CreateRole("CompanyAdmin", context, roleManager);
            CreateRole("GroupAdmin", context, roleManager);
            CreateRole("KitchenAdmin", context, roleManager);
            CreateRole("UserAdmin", context, roleManager);

            SeedDatabase(context, roleManager, userManager);
        }
        
        private static void CreateRole(string name,AppDbContext context, RoleManager<HotelRole> _roleManager)
        {
            if (_roleManager.RoleExistsAsync(name).Result)
                return;
            var role = new HotelRole()
            {
                Name = name
            };
            _roleManager.CreateAsync(role).Wait();
        }
        private static void CreateAdminRole(AppDbContext context, RoleManager<HotelRole> _roleManager, UserManager<HotelUser> _userManager)
        {
            bool roleExists = _roleManager.RoleExistsAsync("Admin").Result;
            if (roleExists)
            {
                return;
            }
            
            var role = new HotelRole()
            {
                Name = "Admin"
            };
            _roleManager.CreateAsync(role).Wait();

            var user = new HotelUser()
            {
                UserName = "admin",
                Email = "admin@default.com",
                HotelId=1
                
            };

            string adminPassword = "Password123";
            var userResult =  _userManager.CreateAsync(user, adminPassword).Result;

            if (userResult.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }

        private static void SeedDatabase(AppDbContext _context, RoleManager<HotelRole> _roleManager, UserManager<HotelUser> _userManager)
        {
            var cat1 = new Categories { Code = "", Name = "Standard", Description = "The Bakery's Standard pizzas all year around.", HotelId = 1 };
            var cat2 = new Categories { Code = "", Name = "Spcialities", Description = "The Bakery's Speciality pizzas only for a limited time.", HotelId = 1 };
            var cat3 = new Categories { Code = "", Name = "News", Description = "The Bakery's New pizzas on the menu.", HotelId = 1 };
            var cat4 = new Categories { Code = "1", Name = "Завтраки", Description = "The Bakery's New pizzas on the menu.", HotelId = 1 };
            var cat5 = new Categories { Code = "2", Name = "Обед", Description = "The Bakery's New pizzas on the menu.", HotelId = 1 };

            var usrGr1 = new UserGroups { Name = "Group1", HotelId = 1 };
            var usrGr2 = new UserGroups { Name = "Group2", HotelId = 1 };
            var usrGr3 = new UserGroups { Name = "Group3", HotelId = 1 };
            var usrGr4 = new UserGroups { Name = "Завтраки", HotelId = 1 };
            var usrGr5 = new UserGroups { Name = "Обед", HotelId = 1 };


            var comp1 = new Hotel { Code = "BASE", Name = "Default" };
            var comp2 = new Hotel { Code = "CABACHOK", Name = "Кабачок" };
            var comps = new List<Hotel>() { comp1, comp2 };
            var cats = new List<Categories>()
            {
                cat1, cat2, cat3,cat4,cat5
            };

            var usrs = new List<UserGroups>()
            {
                usrGr1, usrGr2, usrGr3,usrGr4,usrGr5
            };


            var d1=new Dish { Code = "1", Name = "Борщ", Price = 2,  Description = "...", CategoriesId=4, HotelId = 1 };
            var d2 = new Dish { Code = "2", Name = "Котлета", Price = 2, Description = "A normal pizza with a taste from the forest.", CategoriesId = 4, HotelId = 1 };
            var d3 = new Dish { Code = "3", Name = "Запеканка", Price = 2, Description = "A normal pizza with a taste from the forest.", CategoriesId = 3, HotelId = 1 };
            var d4 = new Dish { Code = "4", Name = "Омлет", Price = 2, Description = "A normal pizza with a taste from the forest.", CategoriesId = 3, HotelId = 1 };


            var dishes = new List<Dish>()
            {
                d1, d2, d3,d4
            };
            //var dc1 = new DishCategory { CategoryId=4,DishId=3};
            //var dc2 = new DishCategory { CategoryId = 5, DishId = 3 };
            //var dc3 = new DishCategory { CategoryId = 5, DishId = 1 };
            //var dishcaetgories = new List<DishCategory>()
            //{
            //    dc1,dc2,dc3

            //};


            var user1 = new HotelUser { UserName = "user1@gmail.com", Email = "user1@gmail.com" };
            var user2 = new HotelUser { UserName = "user2@gmail.com", Email = "user2@gmail.com" };
            var user3 = new HotelUser { UserName = "user3@gmail.com", Email = "user3@gmail.com" };
            var user4 = new HotelUser { UserName = "user4@gmail.com", Email = "user4@gmail.com" };
            var user5 = new HotelUser { UserName = "user5@gmail.com", Email = "user5@gmail.com" };

            string userPassword = "Password123";

            var users = new List<HotelUser>()
            {
                user1, user2, user3, user4, user5
            };

            foreach (var user in users)
            {
                _userManager.CreateAsync(user, userPassword).Wait();
            }






            
            _context.Hotel.AddRange(comps);
            _context.SaveChanges();
            _context.Categories.AddRange(cats);
            _context.UserGroups.AddRange(usrs);


         
           // _context.PizzaIngredients.AddRange(pizIngs);

            _context.Dish.AddRange(dishes);
           // _context.DishCategory.AddRange(dishcaetgories);
            

            _context.SaveChanges();
        }

        private static void ClearDatabase(AppDbContext _context)
        {
            

            var users = _context.Users.ToList();
            var userRoles = _context.UserRoles.ToList();

            foreach (var user in users)
            {
                if (!userRoles.Any(r => r.UserId == user.Id))
                {
                    _context.Users.Remove(user);
                }
            }



            var categories = _context.Categories.ToList();
            _context.Categories.RemoveRange(categories);

            _context.SaveChanges();

            var groupusers = _context.UserGroups.ToList();
            _context.UserGroups.RemoveRange(groupusers);

            _context.SaveChanges();
        }
    }
}
