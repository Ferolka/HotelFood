
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
using System.Text.RegularExpressions;
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
        [Authorize(Roles = "Admin,CompanyAdmin,UserAdmin,GroupAdmin,SubGroupAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserModal([FromForm] UpdateUserModel usermodel, [FromForm] string roles, [FromForm] string companies, IEnumerable<HotelUser> it)
        {

            //string id = User.GetUserId();
            if (!ModelState.IsValid)
                return   PartialView("EditUserModal", usermodel);
            //return await Task.FromResult(Json(new { res = "FAIL", reason = "Error occured! Maybe passwords are mismatching" }));

            _logger.LogInformation("EditUserModal");
            try
            {


                List<string> newRoles = new List<string>();
                List<int> newCompanies = new List<int>();
                if (!string.IsNullOrEmpty(roles))
                    newRoles = roles.Split(",").Select(s => s.Trim()).ToList();
                if (!string.IsNullOrEmpty(companies))
                {
                    try
                    {
                        newCompanies = companies.Split(",").Select(s => int.Parse(s.Trim())).ToList();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("companies list invalid", ex);
                    }
                }
                if (usermodel.IsNew)
                {
                    var creator = await _userManager.FindByIdAsync(User.GetUserId());
                    HotelUser checkmailuser = await _userManager.FindByEmailAsync(usermodel.Email);
                    if (checkmailuser != null)
                    {
                        _logger.LogWarning("Error creating user email already taken: {0} ", checkmailuser.Email);
                        //string text = _localizer.GetLocalizedString("DuplicateEmail");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "email already taken" }));
                    }
                    if (string.IsNullOrEmpty(usermodel.NewPassword))
                    {
                        //ModelState.AddModelError("NewPassword", "You must specify a value");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Password's fields can not be empty" }));
                    }
                    if (string.IsNullOrEmpty(usermodel.ConfirmPassword))
                    {
                        //ModelState.AddModelError("ConfirmPassword", "You must specify a value");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Password's fields can not be empty" }));
                    }
                    if (usermodel.ConfirmPassword != usermodel.NewPassword)
                    {
                        //ModelState.AddModelError("ConfirmPassword", "Incorrect value");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Passwords mismatching" }));
                    }
                    _logger.LogInformation("Creating new User Name={0}, email={1}", usermodel.UserName, usermodel.Email);
                    //CompanyUser usr = new CompanyUser() { CompanyId = User.GetCompanyID() };
                    HotelUser usr = new HotelUser() { HotelId = User.GetHotelID() };
                    usermodel.CopyTo(usr, true);
                    usr.Id = Guid.NewGuid().ToString();
                    if (!usr.UserGroupId.HasValue)
                        usr.UserGroupId = creator.UserGroupId;
                    //if (!usr.UserSubGroupId.HasValue)
                    //    usr.UserSubGroupId = creator.UserSubGroupId;
                    var userResult = await _userManager.CreateAsync(usr, usermodel.NewPassword);

                    if (!userResult.Succeeded)
                    {
                        _logger.LogError("Creating user is not succeeded for user {0}{1}", usermodel.UserName, userResult.ToString());
                        //return await Task.FromResult(Json(new { res = "FAIL", reason = "error occured while creating user" }));
                        usermodel.Errors = userResult.Errors.Select(x => x.Description).ToList();
                        _logger.LogWarning("Error creating user async : {0} ", usr.UserName);
                        return PartialView("EditUserModal", usermodel);
                    }

                    //current  roles
                    var userRoles = await _userManager.GetRolesAsync(usr);
                    //added roles 
                    var addedRoles = newRoles.Except(userRoles);
                    //removed roles
                    var removedRoles = userRoles.Except(newRoles);

                    userResult = await _userManager.AddToRolesAsync(usr, addedRoles);
                    if (!userResult.Succeeded)
                    {
                        _logger.LogError("Creating user is not sucess beacause add role error {0}", userResult.ToString());
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "error occured adding roles to user" }));
                    }

                    //newCompanies.Add(User.GetCompanyID());
                    var userResultCompanies = await _companyuser_repo.AddCompaniesToUserAsync(usr.Id, newCompanies);

                    if (!userResultCompanies)
                    {
                        _logger.LogError("error adding company to user");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "error occured adding company to user" }));
                    }

                    userResult = await _userManager.RemoveFromRolesAsync(usr, removedRoles);
                    if (!userResult.Succeeded)
                    {
                        _logger.LogError("error removing role from user");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "error removing role from user" }));
                    }


                    //usr.ChildrenCount = 1;
                    //if (usr.ConfirmedByAdmin)
                    //{
                    //    var code = await _userManager.GeneratePasswordResetTokenAsync(usr);
                    //    var callbackUrl = Url.Action(
                    //        "SetNewPassword",
                    //        "Account",
                    //        new { userId = usr.Id, code = code },
                    //        protocol: HttpContext.Request.Scheme);
                    //    usr.EmailConfirmed = true;
                    //    await _companyuser_repo.PostUpdateUserAsync(usr, true);
                    //    EmailService emailService = new EmailService();
                    //    await _email.SendEmailNoExceptionAsync(usr.Email, "Скидання паролю",
                    //    $"³Шановний, {usr.NameSurname}<br>" +
                    //    $"Підтвердження паролю до сайту Кабачок.<br>" +
                    //    $"Дані для входу в обліковий запис. <br>" +
                    //    $"Login: {usr.UserName} <br>" +
                    //    $"Перейдіть за посиланням для підтвердження паролю:<a href='{callbackUrl}'> посилання</a><br>" +
                    //    $"" +
                    //    $"" +
                    //    $"<br><br><br>Якщо ви отримали цей лист випадково - проігноруйте його.<br>" +
                    //    $"<h2>У разі виникнення питань звертайтесь на пошту: admin@kabachok.group</h2>");
                    //    //await _email.SendEmailAsync(usermodel.Email, "��������� ��������� ������", _localizer.GetLocalizedString(SafeFormat("SendEmailCreatedByAdmText", usr.NameSurname,usr.UserName)));

                    //}

                    //if (!userResult.Succeeded)
                    //{
                    //    _logger.LogError("Creating user is not sucess {0}", userResult.ToString());

                    //    foreach (var err in userResult.Errors)
                    //    _logger.LogError(err.Description, err.Code);
                    //    return await Task.FromResult(Json(new { res = "FAIL", reason = "Error occured" }));
                    //}
                    var resultUpdateUser = await _companyuser_repo.PostUpdateUserAsync(usr, true);
                    if (!resultUpdateUser)
                    {
                        _logger.LogError("error updating user{0}", usr.UserName);
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "error occured updating user" }));
                    }
                }
                else
                {
                    HotelUser user = await _userManager.FindByIdAsync(usermodel.Id);
                    if (user == null)
                    {
                        return BadRequest();
                    }
                    //var tmp = user.RegisterDate.ToString();
                    //update user child
                    var i = 0;
                    foreach (var reb in it)
                    {
                        // IFormFile filePict = null;
                        var filePict = Request.Form.Files.FirstOrDefault(f => f.Name.StartsWith($"it[{i}]"));

                        for (var idx = 0; idx < Request.Form.Files.Count; idx++)
                        {
                            var fileindex = -1;
                            Regex regex = new Regex(@"\w+\[(?<idx>\d+)\][.]\w+");
                            Match match = regex.Match(Request.Form.Files[idx].Name);

                            if (!match.Success || !int.TryParse(match.Groups["idx"].Value, out fileindex) || fileindex != i)
                            {
                                continue;
                            }
                            filePict = Request.Form.Files[idx];
                            break;
                        }

                        HotelUser user_to_update;
                        user_to_update = user;
                        //if (reb.Id == usermodel.Id)
                        //{
                        //    usermodel.ChildNameSurname = reb.ChildNameSurname;
                        //    usermodel.ChildBirthdayDate = reb.ChildBirthdayDate;
                        //    //usermodel.CopyEditedParamsTo(user);
                        //    user_to_update = user;
                        //}
                        //else
                        //{
                        //    user_to_update = await _userManager.FindByIdAsync(reb.Id);
                        //    if (user_to_update != null)
                        //    {
                        //        user_to_update.ChildNameSurname = reb.ChildNameSurname;
                        //        user_to_update.ChildBirthdayDate = reb.ChildBirthdayDate;
                        //        if (user_to_update.ChildNameSurname != null && user_to_update.ParentUserId != null)
                        //        {
                        //            CompanyUser parent = await _userManager.FindByIdAsync(user_to_update.ParentUserId);
                        //            string translit_text = Translit.cyr2lat(user_to_update.ChildNameSurname);
                        //            user_to_update.UserName = parent.UserName + "_" + translit_text;
                        //        }
                        //    }
                        //}
                        //if (user_to_update == null)
                        //{
                        //    ModelState.AddModelError("", "User Not Found");
                        //    break;
                        //}
                        if (filePict != null)
                        {
                            Pictures pict = _context.Pictures.SingleOrDefault(p => p.Id == user_to_update.PictureId);
                            if (pict != null)
                            {
                                _context.Remove(pict);
                            }
                            pict = null;
                            if (pict == null)
                            {
                                pict = new Pictures();

                                try
                                {
                                    _context.Add(pict);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error adding Picture to database");
                                    ModelState.AddModelError("", "Error adding Picture to database");
                                    return RedirectToAction("Users");
                                }
                            }
                            byte[] data;
                            using (var stream = filePict.OpenReadStream())
                            {
                                byte[] imgdata = new byte[stream.Length];
                                stream.Read(imgdata, 0, (int)stream.Length);
                                //pict.PictureData = imgdata;
                                data = imgdata;
                            }
                            pict.PictureData = data;
                            PicturesController.CompressPicture(pict, pictWidth, pictHeight);

                            if (_context.Entry(pict).State != EntityState.Added)
                            {


                                _context.Update(pict);
                            }
                            await _context.SaveChangesAsync();
                            user_to_update.PictureId = pict.Id;

                        }
                        if (reb.Id != usermodel.Id)
                        {
                            try
                            {
                                IdentityResult rebResult = await _userManager.UpdateAsync(user_to_update);
                                if (!rebResult.Succeeded)
                                {
                                    return await Task.FromResult(Json(new { res = "FAIL", reason = "Some error occured" }));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error Update Child");
                                ModelState.AddModelError("", ex.Message);
                                return RedirectToAction("Users");
                            }
                        }
                        i++;
                    }
                    //end of update

                    //usermodel.CopyEditedModalDataTo(user);
                    //var userResult = await _userManager.UpdateAsync(user);

                    //if (user != null)
                    //{
                    //   await UserFinance();
                    //}
                    HotelUser checkuserIfexistAlreadyMail = await _userManager.FindByEmailAsync(usermodel.Email);
                    HotelUser checkuserIfexistAlreadyLogin = await _userManager.FindByNameAsync(usermodel.UserName);
                    if (checkuserIfexistAlreadyMail != null && (usermodel.Email != user.Email))
                    {
                        _logger.LogWarning("Error editing user email already taken: {0} ", checkuserIfexistAlreadyMail.Email);
                        //string text = _localizer.GetLocalizedString("DuplicateEmail");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "email already taken" }));
                    }
                    if (checkuserIfexistAlreadyLogin != null && (usermodel.UserName != user.UserName))
                    {
                        _logger.LogWarning("Error editing user email already taken: {0} ", checkuserIfexistAlreadyLogin.UserName);
                        //string text = _localizer.GetLocalizedString("DuplicateEmail");
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "login already taken" }));
                    }
                    if (usermodel.IsPasswordChanged)
                    {

                        if (!usermodel.NewPassword.Equals(usermodel.ConfirmPassword))
                        {
                            _logger.LogWarning("Change password,  passwords mismatch");
                            return await Task.FromResult(Json(new { res = "FAIL", reason = "password mismatch" }));
                        }
                        if (usermodel.NewPassword.Equals(usermodel.ConfirmPassword))
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var result = await _userManager.ResetPasswordAsync(user, token, usermodel.NewPassword);
                            if (result.Succeeded)
                            {
                                _logger.LogWarning("Update user password,  new password for user {0} was applied", user.UserName);
                                //return await Task.FromResult(Json(new { res = "INFO", reason = "Password applied, now click green save button" }));
                            }
                            else
                            {
                                usermodel.Errors = result.Errors.Select(x => x.Description).ToList();
                                _logger.LogWarning("Error updating password for user: {0} ", user.UserName);
                                return PartialView(usermodel);
                            }
                        }
                    }
                    //if (!user.ConfirmedByAdmin && usermodel.ConfirmedByAdmin)
                    //{
                    //    user.EmailConfirmed = true;
                    //    await _companyuser_repo.PostUpdateUserAsync(user, true);
                    //    string email = usermodel.Email;
                    //    if (user.ParentUserId != null)
                    //    {
                    //        var parent = await _userManager.FindByIdAsync(user.ParentUserId);
                    //        email = parent.Email;
                    //    }

                    //    EmailService emailService = new EmailService();
                    //    await _email.SendEmailAsync(email, "Підтвердження облікового запису",
                    //        $"Вітаю, {user.NameSurname}<br>" +
                    //        $"Ваш аккаунт було підтверджено адміністратором!<br>" +
                    //        $"Наразі вам доступний весь функціонал.<br>" +
                    //        $"" +
                    //        $"" +
                    //        $"<br><br><br>Якщо ви отримали цей лист випадково - проігноруйте його..<br>" +
                    //        $"<h2>У разі виникнення питань звертайтесь: admin@kabachok.group</h2>");
                    //}
                    usermodel.CopyEditedModalDataTo(user);
                    var userResult = await _userManager.UpdateAsync(user);
                    if (!userResult.Succeeded)
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Some error occured" }));
                    //current  roles
                    var userRoles = await _userManager.GetRolesAsync(user);
                    //added roles 
                    var addedRoles = newRoles.Except(userRoles);
                    //removed roles
                    var removedRoles = userRoles.Except(newRoles);

                    userResult = await _userManager.AddToRolesAsync(user, addedRoles);

                    if (!userResult.Succeeded)
                    {
                        _logger.LogWarning("Add roles to user error: {0} ", user.UserName);
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Add roles to user error occured" }));
                    }

                    var addCompaniesToUser = await _companyuser_repo.AddCompaniesToUserAsync(user.Id, newCompanies);
                    if (!addCompaniesToUser)
                    {
                        _logger.LogWarning("Add companies to user error: {0} ", user.UserName);
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Error occured adding roles to user" }));
                    }

                    userResult = await _userManager.RemoveFromRolesAsync(user, removedRoles);

                    if (!userResult.Succeeded)
                    {
                        return await Task.FromResult(Json(new { res = "FAIL", reason = "Error occured! Refresh the page and try again" }));
                    }
                    await _companyuser_repo.PostUpdateUserAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EditUser");
                ModelState.AddModelError("", ex.Message);
                return PartialView("EditUserModal", usermodel);
            }
            return this.UpdateOk();

        }
    }
}
