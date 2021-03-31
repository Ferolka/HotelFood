
using HotelFood.Core;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICompanyUserRepository _companyuser_repo;
        private readonly SharedViewLocalizer _localizer;

        private const int pictWidth = 200;
        private const int pictHeight = 300;
        public AccountController(AppDbContext context, UserManager<HotelUser> userManager,
                                 SignInManager<HotelUser> signInManager,
                                 ILogger<HotelUser> logger, ICompanyUserRepository companyuser_repo, SharedViewLocalizer localizer)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _companyuser_repo = companyuser_repo;
            _localizer = localizer;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult LoginModal(string returnUrl)
        {
            return PartialView("LoginModal", new LoginViewModel
            {
                ReturnUrl = returnUrl,
                IsRemember = true
            }); ;
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.IsModal)
                {
                    return View("LoginModal", model);
                }

                else
                {
                    return View(model);
                }

            }
            _logger.LogInformation("User {0} is going to login ", model.UserName);

            var user = await _userManager.FindByNameAsync(model.UserName.ToLower());

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.UserName.ToLower());
            }
            if (user != null && user.EmailConfirmed)
            {
                //var claims = await _userManager.GetClaimsAsync(user);
                // claims.Add(new System.Security.Claims.Claim("companyid", "44"));

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.IsRemember, true);

                if (result.Succeeded)
                {
                    var validation = _companyuser_repo.ValidateUserOnLogin(user);
                    if (validation > 0)   // required refresh of claims
                    {
                        await _signInManager.RefreshSignInAsync(user);
                    }
                    if (validation < 0)   // probably not allow to login
                    {

                    }
                    if (model.IsModal)
                    {
                        return Ok(new { res = "OK", returnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? Url.Action("Index", "Home") : model.ReturnUrl });
                        //Task.FromResult(Json(new { res="OK",ReturnUrl= string.IsNullOrEmpty(model.ReturnUrl) ? Url.Content("~") : model.ReturnUrl }))
                    }
                    if (string.IsNullOrEmpty(model.ReturnUrl))
                        return RedirectToAction("Index", "Home");

                    return Redirect(model.ReturnUrl);
                }
                //if(user.AccessFailedCount >= 3)
                //{
                //    ModelState.AddModelError("", "Contact to admin to unlock your account");
                //}
                //user.AccessFailedCount += 1;
                //await _companyuser_repo.PostUpdateUserAsync(user, true);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", _localizer.GetLocalizedString("UserLockedOut"));
                    _logger.LogWarning("The  user {0} is Locked out", model.UserName);

                }
                else
                {
                    ModelState.AddModelError("", _localizer.GetLocalizedString("IncorrectPassword"));
                    _logger.LogWarning("The password for user {0} is invalid", model.UserName);
                }
                _logger.LogWarning("The password for user {0} is invalid", model.UserName);
                return View("LoginModal", model);
            }
            if (user != null && !user.EmailConfirmed)
            {
                _logger.LogWarning("User: {0} hasn't confirmed Email: {1}", model.UserName, user.Email);
                ModelState.AddModelError("", _localizer.GetLocalizedString("You have to confirm your Email before"));
                return View("LoginModal", model);
            }
            if (user == null)
            {
                _logger.LogWarning("Can't find registered user {0}", model.UserName);
                ModelState.AddModelError("", _localizer.GetLocalizedString("UserNotFound"));
                return View("LoginModal", model);
            }

            if (model.IsModal)
                return PartialView("LoginModal", model);
            else
                return View(model);

        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin,GroupAdmin,SubGroupAdmin")]
        public IActionResult Users()//async Task<IActionResult> Users()
        {
            // return View(await _userManager.Users.Where(u => u.CompanyId == User.GetCompanyID()).ToListAsync());
            return View(new List<HotelUser>());
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
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin")]
        public IActionResult CreateUserModal()
        {

            var user = new UpdateUserModel();
            user.InitializeNew();
            if (user == null)
            {
                return NotFound();
            }
     
            return PartialView("EditUserModal", user);
        }

        [Authorize]
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin,GroupAdmin,SubGroupAdmin")]
        public async Task<IActionResult> EditUserModal(string userId)
        {

            //string id = User.GetUserId();
            HotelUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            

            //ViewData["UserGroupId"] = new SelectList(_companyuser_repo.GetUserGroups(User.GetCompanyID()).Result, "Id", "Name", user.UserGroupId);
            //ViewData["UserSubGroupId"] = new SelectList(_companyuser_repo.GetUserSubGroups(User.GetCompanyID()).Result, "Id", "Name", user.UserSubGroupId);


            //ViewData["UserType"] = EnumHelper<UserTypeEnum>.GetSelectListWithIntegerValues(user.UserTypeEn,_localizer).ToList() ;
            var model = _companyuser_repo.GetUpdateUserModel(user);
            model.AutoLoginUrl = Url.Action("AutoLogon", "Account", new { token = model.AutoLoginToken, username = model.UserName }, Request.Scheme);

            //return PartialView(model);
            return PartialView("EditUserModal", model);

        }
        [Authorize]
        public JsonResult UserCompanies()
        {
            return Json(_companyuser_repo.GetCurrentUsersCompaniesAsync(User.GetUserId()).Result);
        }
        [Authorize]
        public JsonResult UserOtherCompanies()
        {
            return Json(_companyuser_repo.GetCurrentUsersCompaniesAsync(User.GetUserId()).Result.Where(c => c.Id != User.GetHotelID()));
        }
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin")]
        public JsonResult UserRoles(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
                return new JsonResult(null) { StatusCode = 500 };
            return Json(_userManager.GetRolesAsync(user).Result);
        }
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin")]
        public JsonResult ErrorPasswChange()
        {
            return new JsonResult(null) { StatusCode = 424 };
        }
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin")]
        public async Task<IActionResult> RolesForUser(string userId)
        {

            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null && !string.IsNullOrEmpty(userId))
                return NotFound();
            var roles = await _companyuser_repo.GetRolesForUserAsync(user);
            return PartialView(roles);
        }

        [Authorize]
        public async Task<IActionResult> CompaniesForUser(string userId)
        {

            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null && !string.IsNullOrEmpty(userId))
                return NotFound();
            var usercompanies = await _companyuser_repo.GetAssignedCompaniesEdit(userId);
            return PartialView(usercompanies);
        }
        [Authorize]
        public async Task<IActionResult> EditCompaniesForUser(string userId)
        {

            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null && !string.IsNullOrEmpty(userId))
                return NotFound();
            var usercompanies = await _companyuser_repo.GetAssignedEditCompanies(userId);
            return PartialView(usercompanies);
        }
        [Authorize/*(Roles = "Admin,CompanyAdmin,UserAdmin")*/]
        public async Task<IActionResult> SetCompanyId(int CompanyId)
        {
            if (!await _companyuser_repo.ChangeUserCompanyAsync(User.GetUserId(), CompanyId, User))
                return BadRequest();
            var user = await _userManager.FindByIdAsync(User.GetUserId());
            if (user == null)
                return BadRequest();
            await _signInManager.RefreshSignInAsync(user);
            return new EmptyResult();//RedirectToAction("Index", "Home");
        }
    }
}
