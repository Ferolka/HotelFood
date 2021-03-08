
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace HotelFood.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<HotelUser> _userManager;
        private readonly SignInManager<HotelUser> _signInManager;
        private readonly ILogger<HotelUser> _logger;
        private readonly IHotelUserRepository _companyuser_repo;
        
        private const int pictWidth = 200;
        private const int pictHeight = 300;
        public AccountController(AppDbContext context, UserManager<HotelUser> userManager,
                                 SignInManager<HotelUser> signInManager,
                                 ILogger<HotelUser> logger, IHotelUserRepository companyuser_repo)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _companyuser_repo = companyuser_repo;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("Account/Users/UsersList")]
        [Route("Account/UsersList")]
        public async Task<IActionResult> UsersList(QueryModel querymodel)
        {

            // var query = _userManager.Users;

            ViewData["QueryModel"] = querymodel;
            var query = await GetQueryListUsers(querymodel, 20);
            //if (querymodel.RelationFilter > 0)
            //{
            //    query = query.Where(d => d.CategoriesId == querymodel.RelationFilter);
            //}



            //return PartialView(await _userManager.Users.Where(u => u.CompanyId == User.GetCompanyID()).ToListAsync());
            return PartialView(query);
        }
        public async Task<List<HotelUser>> GetQueryListUsers(QueryModel querymodel, int pageRecords, bool loadchilds = true)
        {
            ViewData["QueryModel"] = querymodel;
            var query = _userManager.Users.
                Include(u => u.HotelGuests)
                .Where(d => string.IsNullOrEmpty(querymodel.SearchCriteria) ||
                        d.Email.Contains(querymodel.SearchCriteria) ||
                        d.UserName.Contains(querymodel.SearchCriteria) ||
                        d.NameSurname.Contains(querymodel.SearchCriteria));
            
            if (!string.IsNullOrEmpty(querymodel.SortField))
            {
                query = query.OrderBy(querymodel.SortField, querymodel.SortOrder);
            }
            if (querymodel.Page > 0)
            {
                query = query.Skip(pageRecords * querymodel.Page);
            }
            if (pageRecords > 0)
                query = query.Take(pageRecords);
            List<HotelUser> userslist = await query.ToListAsync();
            
            return userslist;
        }
    }
}
