using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Http;
using HotelFood.Core;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Runtime.CompilerServices;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.ViewModels;

namespace HotelFood.Repositories
{
    public class CompanyUserRepository : ICompanyUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelUser> _logger;
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<HotelRole> _roleManager;
        private readonly IMemoryCache _cache;
        private readonly SignInManager<HotelUser> _signInManager;
        public CompanyUserRepository(AppDbContext context, ILogger<HotelUser> logger,
            UserManager<HotelUser> userManager, IMemoryCache cache, RoleManager<HotelRole> rolemanager,
             SignInManager<HotelUser> signInManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _cache = cache;
            _roleManager = rolemanager;
            _signInManager = signInManager;


        }
        public string GetCurrentCompany()
        {

            var company = _cache.GetCachedCompanyAsync(_context, _context.CompanyId).Result;
            if (company == null && _context.CurrentUser != null)
            {
                var user = _userManager.FindByIdAsync(_context.CurrentUser.GetUserId()).Result;
                if (user != null)
                    PostUpdateUserAsync(user).Wait();
                return string.Empty;
            }
            return company.Name;
        }
        public decimal GetUserBalance()
        {
            if (_context.CurrentUser != null)
            {

                var fin = _context.UserFinances.Where(uf => uf.Id == _context.CurrentUser.GetUserId()).FirstOrDefault();
                if (fin != null)
                    return fin.Balance;
                var user = _userManager.FindByIdAsync(_context.CurrentUser.GetUserId()).Result;
                if (user != null)
                    PostUpdateUserAsync(user).Wait();

            }
            return 0;
        }
        public async Task<decimal> GetUserBalanceAsync()
        {
            if (_context.CurrentUser != null)
            {

                var fin = await _context.UserFinances.Where(uf => uf.Id == _context.CurrentUser.GetUserId()).FirstOrDefaultAsync();
                if (fin != null)
                    return fin.Balance;
                var user = await _userManager.FindByIdAsync(_context.CurrentUser.GetUserId());
                if (user != null)
                    await PostUpdateUserAsync(user);

            }
            return 0;
        }
        public async Task<List<Hotel>> GetCurrentUsersCompaniesAsync(string userId)
        {

            return await _context.HotelGuests.IgnoreQueryFilters().Include(c => c.Hotel).Where(cu => cu.HotelUserId == userId).
                Select(c => c.Hotel).ToListAsync();
        }
        public async Task<List<Hotel>> GetCompaniesAsync()
        {

            return await _cache.GetCachedCompaniesAsync(_context);
        }
        public async Task<List<AssignedCompanyEditViewModel>> GetAssignedCompaniesEdit(string userId)
        {
            var assigned = await GetCurrentUsersCompaniesUserAsync(userId);
            var cur = await GetCurrentUserCompanie(userId);
            var model = (await GetCompaniesAsync()).AsQueryable().Select(c => new AssignedCompanyEditViewModel
            {
                CompanyID = c.Id,
                CompanyName = c.Name,
                IsAssigned = false
            }).ToList(); ;

            model.ForEach(m => { m.IsAssigned = assigned.Any(c => c.HotelId == m.CompanyID); m.IsCurrent = cur.Any(c => c.HotelId == m.CompanyID); }); 
            return model;
        }
        public async Task<List<AssignedCompanyEditViewModel>> GetAssignedEditCompanies(string userId)
        {
            var assigned = await GetCurrentUsersCompaniesUserAsync(userId);
            //var cur = await GetCurrentUserCompanie(userId);
            var model = (await GetCompaniesAsync()).AsQueryable().Select(c => new AssignedCompanyEditViewModel
            {
                CompanyID = c.Id,
                CompanyName = c.Name,
                IsAssigned = false
            }).ToList(); ;

            model.ForEach(m =>  m.IsAssigned = assigned.Any(c => c.HotelId == m.CompanyID));
            return model;
        }
        public async Task<List<HotelUser>> GetCurrentUserCompanie(string userId)
        {

            return await _context.Users.Where(cu => cu.Id == userId).ToListAsync();

        }
        public async Task<List<HotelGuests>> GetCurrentUsersCompaniesUserAsync(string userId)
        {

            return await _context.HotelGuests.IgnoreQueryFilters().Where(cu => cu.HotelUserId == userId).ToListAsync();

        }
        public async Task<HotelGuests> GetCurrentUserCompaniesUserAsync(string userId)
        {

            return await _context.HotelGuests.FirstOrDefaultAsync(cu => cu.HotelUserId == userId);

        }
        public async Task<HotelGuests> GetUserCompaniesUserAsync(string userId,int companyId)
        {

            return await _context.HotelGuests.IgnoreQueryFilters().FirstOrDefaultAsync(cu => cu.HotelUserId == userId && cu.HotelId==companyId);

        }
        public async Task<int> GetUserCompanyCount(string userId)
        {
            return await _context.HotelGuests.IgnoreQueryFilters().Where(cu => cu.HotelUserId == userId).CountAsync();
        }
        public async Task<bool> ChangeUserCompanyAsync(string userId, int companyid, ClaimsPrincipal claims)
        {
            //if (companyid == _context.CompanyId)
            //    return true;
            //var newcompany = (await GetCurrentUsersCompaniesAsync(userId)).FirstOrDefault(c => c.Id == companyid);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            if (user.HotelId == companyid)
            {
                _logger.LogWarning($"Attempt to change company for user {user.Id} to the same {companyid}. Something wrong");
                return true;
            }
                
            var currentcompany = await GetCurrentUserCompaniesUserAsync(userId );
            var newcompany = await GetUserCompaniesUserAsync(userId, companyid);
            if (newcompany == null)
                return false;


            try
            {
                if (currentcompany!=null && ( currentcompany.UserGroupId != user.UserGroupId))
                {
                    currentcompany.UserGroupId = user.UserGroupId;
                    //currentcompany.UserSubGroupId = user.UserSubGroupId;
                    _context.Update(currentcompany);
                    await _context.SaveChangesAsync();
                }
                else if (currentcompany == null)
                {
                    _logger.LogWarning($"Company with ID={user.HotelId} detached from user {user.Id} as result user references to group are lost");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error change groups");
            }
            user.HotelId = companyid;
            user.UserGroupId = newcompany.UserGroupId;
            
            _context.Update(user);
            await _context.SaveChangesAsync();
            if(claims!=null)
                CustomClaimsPrincipalFactory.ChangeCompanyId(claims, companyid);

            return true;
        }

        public async Task<List<UserGroups>> GetUserGroups(int companyId)
        {
            return await _context.UserGroups.ToListAsync();
        }
        //public async Task<List<UserSubGroup>> GetUserSubGroups(int companyId)
        //{
        //    return await _context.UserSubGroups.ToListAsync();
        //}
        public async Task<List<UserRoleViewModel>> GetRolesForUserAsync(HotelUser user)
        {
            var userroles = new List<string>();
            if (user != null)
                userroles = (await _userManager.GetRolesAsync(user)).ToList();
            return await _roleManager.Roles.Select(r => new UserRoleViewModel()
            {
                RoleName = r.Name,
                IsAssigned = userroles.Contains(r.Name),//_userManager.IsInRoleAsync(user,r.Name).Result,
                userId = user == null ? "" : user.Id
            }).ToListAsync();
        }
        public async Task<bool> AddCompaniesToUserAsync(string userid, IList<int> companiesIds)
        {
            try
            {

                var newrecords = companiesIds.Select(it => new HotelGuests() { HotelId = it, HotelUserId = userid }).ToList();
                var existing = await GetCurrentUsersCompaniesUserAsync(userid);

                var deleted = existing.Where(it => !newrecords.Any(n => n.HotelId == it.HotelId));
                var added = newrecords.Where(it => !existing.Any(n => n.HotelId == it.HotelId));//.Except(existing);
                await _context.AddRangeAsync(added);
                _context.RemoveRange(deleted);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("AddCompaniesToUserAsync", ex);
                return false;
            }
            return true;
        }
        public int GetDefaultCompanyId()
        {
            var comp = _cache.GetCachedCompaniesAsync(_context).GetAwaiter().GetResult().FirstOrDefault(c => c.IsDefault.HasValue && c.IsDefault.Value);
            if (comp != null)
                return comp.Id;
            comp = _cache.GetCachedCompaniesAsync(_context).GetAwaiter().GetResult().FirstOrDefault();
            if (comp != null)
                return comp.Id;
            return 0;
        }
        public bool IsCompanyExist(int companyId)
        {
            return _cache.GetCachedCompaniesAsync(_context).Result.Any(c => c.Id == companyId);
        }
        public async Task<int> GetDefaultCompanyForUser(HotelUser user)
        {
            var existing_companies = await GetCurrentUsersCompaniesUserAsync(user.Id);
            if (IsCompanyExist(user.HotelId))
                return user.HotelId;
            return GetDefaultCompanyId();

        }
        public async Task<bool> CheckUserFinanceAsync(HotelUser user,int companyId)
        {
            try
            {
                var fin = _context.UserFinances.Where(f => f.Id == user.Id).FirstOrDefault();
                if (fin == null)
                {
                    fin = new UserFinance() { Id = user.Id, HotelId = companyId };
                    _context.Add(fin);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"CheckUserFinance");
                return false;
            }
            return true;
        }
        public async Task<bool> PostUpdateUserAsync(HotelUser user, bool isNew = false)
        {
            bool res = true;
            try
            {

                var existing_companies = await GetCurrentUsersCompaniesUserAsync(user.Id);
                if (!IsCompanyExist(user.HotelId))
                {
                    _logger.LogWarning($"Something wrong with user {user.Id} attached to  not existing company {user.HotelId}");
                    user.HotelId = await GetDefaultCompanyForUser(user);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }

                if (existing_companies.Count == 0)
                {
                    _logger.LogWarning($"Something wrong user {user.Id} are not assigned to any company ");

                     res &= await AddCompaniesToUserAsync(user.Id, (new[] { user.HotelId }).ToList());
                    _logger.LogWarning($"Assigning user {user.Id} to company {user.HotelId } ");
                    existing_companies= await GetCurrentUsersCompaniesUserAsync(user.Id);

                }
                var current_companyrecord = existing_companies.FirstOrDefault(c => c.HotelId == user.HotelId);
                if(current_companyrecord==null && existing_companies.Count>0)
                {

                    var newcompany = existing_companies.FirstOrDefault().HotelId;
                    _logger.LogWarning($"Changing  user {user.Id} default company to {newcompany } ");
                    if (ChangeUserCompanyAsync(user.Id, newcompany, null).GetAwaiter().GetResult())
                    {
                        current_companyrecord = existing_companies.FirstOrDefault(c => c.HotelId == newcompany);
                    }
                    else
                    {
                        _logger.LogError("ChangeUserCompanyAsync return false");
                    }
                }
                //var current_companyrecord = await GetCurrentUserCompaniesUserAsync(user.Id);
                if(current_companyrecord!=null && (current_companyrecord.UserGroupId != user.UserGroupId ))
                {
                    current_companyrecord.UserGroupId = user.UserGroupId;
                  //  current_companyrecord.UserSubGroupId = user.UserSubGroupId;
                    await _context.SaveChangesAsync();

                }
               // res &= await CheckUserFinanceAsync(user, user.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Post update user", ex);
                return false;
            }
            return res;
        }
        //public async Task<List<HotelUser>> GetUserChilds(string userId, int companyId, bool onlyChild = false)
        //{
        //    //  var user = await _userManager.FindByIdAsync(userId);
        //    //return await _userManager.Users.Where(u => u.CompanyId == companyId && u.ParentUserId == userId || (u.Id == userId && onlyChild) || u.Id == user.ParentUserId).ToListAsync();
        //    var child = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
        //    if (child.IsChild())
        //    {
        //        userId = child.ParentUserId;
        //    }
        //    var childs = await _userManager.Users.Where(u => u.CompanyId == companyId && u.ParentUserId == userId 
        //                        || (u.Id == userId && !onlyChild) )
        //                        .OrderByDescending(u=> (u.Id== userId?1:0))  //parent always first
        //                        .ToListAsync();
        //    //add day limit to model
        //    childs.ForEach(ch =>
        //    {
        //        ch.DayLimit = _context.UserFinances.FirstOrDefault(c => c.Id == ch.Id).DayLimit;
        //    });
        //   // childs.Add(user);
        //    return childs;
        //}
        //public async Task<bool> PostUpdateChildUserAsync(HotelUser childuser, HotelUser parentuser)
        //{
        //    if (string.IsNullOrEmpty(childuser.ParentUserId) || !string.IsNullOrEmpty(parentuser.ParentUserId))
        //        return true; // nothing to do
        //    if (childuser.CompanyId != parentuser.CompanyId)
        //    {
        //        childuser.CompanyId = parentuser.CompanyId;
        //        _context.Update(childuser);
        //        await _context.SaveChangesAsync();
        //    }
        //    var companies = await GetCurrentUsersCompaniesUserAsync(parentuser.Id);
        //    var child_companies = companies.Select(c => c.CompanyId).ToList();
        //    return await AddCompaniesToUserAsync(childuser.Id, child_companies);
        //}
        //public async Task<AddBalanceViewModel> AddBalanceViewAsync(string userId)
        //{
        //    //var user = await _context.Users.Include(u => u.UserFinance).FirstOrDefaultAsync(u => u.Id == userId);
        //    var userfin = await _context.UserFinances.FirstOrDefaultAsync(u => u.Id == userId);
        //    if (userfin == null)
        //        return null;
        //    AddBalanceViewModel model = new AddBalanceViewModel()
        //    {
        //        UserId = userId,
        //        CurrentBalance = userfin.Balance,
        //        AmountToAdd = 0,
        //        TotalPreOrderedAmount= userfin.TotalPreOrderedAmount,
        //        TotalPreOrderBalance =userfin.TotalPreOrderBalance
        //    };
        //    return model;
        //}

        //public async Task<bool> AddNewUserChild(string userId, int companyId)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //    if (user == null)
        //    {
        //        _logger.LogError("AddNewUserChild Error user {0} not exists", userId);
        //        return false;
        //    }
        //    int childCount = user.ChildrenCount;
        //    string add_name = "child" + (childCount + 1).ToString();
        //    CompanyUser usr = new CompanyUser() { CompanyId = companyId };
        //    usr.Id = Guid.NewGuid().ToString();
        //    string ticks= DateTime.Now.Ticks.ToString();
        //    //string translit_text = Translit.cyr2lat(user.ChildNameSurname);
        //    // usr.UserName = user.UserName + "_" + translit_text;
        //    usr.UserName = add_name +"_"+user.UserName ;
        //    var resultUser = _userManager.FindByNameAsync(usr.UserName).Result;
        //    if (resultUser != null)
        //    {
        //        usr.UserName = user.UserName + "_" + "child" + "_" + ticks;
        //    }
        //    // usr.Email = ticks+"_"+user.Email;
        //    usr.Email = add_name + "_" + user.Email;
        //    var resultUserMail = _userManager.FindByEmailAsync(usr.Email).Result;
        //    if (resultUserMail != null)
        //    {
        //        usr.Email = user.Email + "_" + "child" + "_" + ticks;
        //    }
        //    usr.ParentUserId = userId;
        //    var userResult = await _userManager.CreateAsync(usr, /*this is password for child*/"PWD" + userId);

        //    if (!userResult.Succeeded)
        //    {
        //        _logger.LogError("Creating new children error {0}", userResult.ToString());
        //        return false;
        //    }
        //    try
        //    {
        //        await PostUpdateUserAsync(usr, true);
        //        await CheckUserFinanceAsync(usr, companyId);
        //        user.ChildrenCount = _context.Users.Where(u => u.ParentUserId == userId).Count() + 1;
        //        _context.Update(user);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("AddNewUserChild error", ex);
        //        return false;
        //    }
        //    return true;
        //}
        public string GetTokenForUser(HotelUser user)
        {
            return user.Id;
        }
        public string GenerateNewCardToken(string userid, string cardUid, bool addHash = false)
        {
            var result = Guid.NewGuid().ToString();
            if (addHash)
            {
                var hashbase = result + cardUid + userid;
                var md5 = System.Security.Cryptography.MD5.Create();
                var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(hashbase));
                result += Convert.ToBase64String(hash);


            }
            return result;

        }
        public async Task<bool> SaveUserCardTokenAsync(string userId, string token)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                user.CardTag = token;
                _context.Update(user);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("SaveUserCardToken", ex);
                return false;
            }
            return true;
        }

        //public async Task<UserSubGroupViewModel> GetSubGroupTree(int companyId)
        //{
        //    var query = await _context.UserSubGroups.ToListAsync();
        //    var tree = new UserSubGroupViewModel();
        //    tree.BuildFrom(query);
        //    return tree;
        //}

        //public async Task<List<int>> UserPermittedSubGroups(string userId, int companyid)
        //{
        //    var res = new List<int>();
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //    if (user == null)
        //        return res;
        //    string sb = "WITH RecursiveQuery (ID, ParentID, Name,CompanyId) \r\n" + 
        //                    "AS \r\n" +
        //                    "( \r\n" +
        //                    " SELECT ID, ParentID, Name,CompanyId \r\n" +
        //                    " FROM UserSubGroups usb where   usb.parentid is not null \r\n" +  //! check here if we need parent
        //                    " UNION ALL \r\n" +
        //                    " SELECT usb.ID, usb.ParentID, usb.Name,usb.CompanyId \r\n" +
        //                    " FROM UserSubGroups usb \r\n" +
        //                    " JOIN RecursiveQuery rec ON usb.ParentID = rec.ID and usb.CompanyId=rec.CompanyId \r\n" +
        //                    ") \r\n" +
        //                    "SELECT DISTINCT ID, ParentID, Name,CompanyId \r\n " +
        //                    "FROM RecursiveQuery  ";
        //    sb += $"where CompanyId={companyid} ";
        //    if (user.UserGroupId.HasValue)
        //    {
        //        //sb += $" AND ( ParentId= {user.UserGroupId.Value}  OR  Id={user.UserGroupId.Value})";
        //        sb += $" AND ( ParentId= {user.UserSubGroupId.Value}  OR  Id={user.UserSubGroupId.Value})";
        //    }

        //    try
        //    {
        //        var query = await _context.UserSubGroups.FromSqlRaw(sb).IgnoreQueryFilters().ToListAsync();
        //        res= query.Select(it => it.Id).ToList();
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //    return res;
        //}
        //public int GetUserSubGroupId(string userId)
        //{
        //    try
        //    {
        //        var userSubGroup = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
        //        return (int)userSubGroup.UserSubGroupId;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,"Get UserSubGroupId {0}", userId);
        //        return 0;
        //    }
        //}
        //public string GetUserSubGroupName(int subgroupid)
        //{
        //    try
        //    {
        //        var userSubGroup=_context.UserSubGroups.FirstOrDefault(usb => usb.Id== subgroupid);
        //        if (userSubGroup != null)
        //            return userSubGroup.Name;
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,"GetUserSubGroupName error");
        //        return string.Empty;
        //    }
        //}
        //public int GetTopLevelSubGroup()
        //{
        //    try
        //    {
        //        var userSubGroup = _context.UserSubGroups.FirstOrDefault(usb=>!usb.ParentId.HasValue);
        //        if (userSubGroup != null)
        //            return userSubGroup.Id;
        //        return -1;


        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "GetTopLevelSubGroup error");
        //        return -1;
        //    }
        //}
        //public List<int> GetUserSubGroups(string userId, int companyid)
        //{
        //    try
        //    {
        //        var userSubGroup = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
        //        if (userSubGroup == null)
        //        {
        //            return new List<int>();
        //        }
        //       var allGroups = _context.UserSubGroups.Where(us=> us.CompanyId==companyid).ToList();
        //        List<int> res = new List<int>();
        //        var first = allGroups.Where(u => u.Id == userSubGroup.UserSubGroupId).First();
        //        int id = (int)userSubGroup.UserSubGroupId;
        //        var start = new List<UserSubGroup>();
        //        start.Add(first);
        //        var ug = allParents(id, allGroups,start);
        //        //ug.ForEach(gr =>
        //        //{
        //        //    res.Add(gr.Id);
        //        //});
        //        res = ug.Select(ug => ug.Id).ToList();
        //        res = res.Distinct().ToList();
        //        return res;

        //    }
        //    catch(Exception ex)
        //    {
        //        _logger.LogError(ex,"Get UserSubGroup {0}", userId);
        //        return new List<int>();
        //    }

        //}

        //private List<UserSubGroup> allParents(int i,List<UserSubGroup> allGroups,List<UserSubGroup> res)
        //{
        //    allGroups.ForEach(us => {
        //        if(us.ParentId == i)
        //        {
        //            res.Add(us);
        //           res.AddRange( allParents(us.Id, allGroups, res));
        //        }
        //        });
        //    return res;
        //}
        public UpdateUserModel GetUpdateUserModel(HotelUser user)
        {
            var res = new UpdateUserModel(user);
            res.AutoLoginToken = _userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth").Result;

            var company = _cache.GetCachedCompanyAsync(_context, user.HotelId).Result;
            if (company != null)
                res.CompanyName = company.Name;
            return res;
        }
        public async Task<bool> ValidateBasicAuthAsync(string val)
        {
            var authHeader = AuthenticationHeaderValue.Parse(val);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return false;
            var res = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return res.Succeeded;
        }

        //public List<SelectListItem> GetUserSubgroupsdWithEmptyList()
        //{
        //    List<SelectListItem> res = _context.UserSubGroups.AsNoTracking()
        //          .OrderBy(n => n.Id ).Select(n =>
        //              new SelectListItem
        //              {
        //                  Value = n.Id.ToString(),
        //                  Text = n.Name
        //              }).ToList();
        //    var empty = new SelectListItem() { Value = "", Text = string.Empty };
        //    res.Insert(0, empty);
        //    return res;
        //}
        public List<SelectListItem> GetCompaniesWithEmptyList()
        {
            List<SelectListItem> res = _cache.GetCachedCompaniesAsync(_context).GetAwaiter().GetResult()
                  .OrderBy(n => n.Id).Select(n =>
                     new SelectListItem
                     {
                         Value = n.Id.ToString(),
                         Text = n.Name
                     }).ToList();
            var empty = new SelectListItem() { Value = "", Text = string.Empty };
            res.Insert(0, empty);
            return res;
        }

        public int ValidateUserOnLogin(HotelUser user)
        {
            try
            {
                var companies = GetCurrentUsersCompaniesUserAsync(user.Id).GetAwaiter().GetResult();
                var current_company = companies.FirstOrDefault(c => c.HotelId == user.HotelId);
                if (current_company == null)
                {
                    _logger.LogWarning($"Attempt to login user {user.Id} to detached company {user.HotelId}");
                    var defaultcompany = GetDefaultCompanyForUser(user).GetAwaiter().GetResult();
                    if (defaultcompany == 0)
                    {
                        _logger.LogWarning($"Not possible to obtain default company for user {user.Id}");
                        return -1;

                    }
                    var changeres = ChangeUserCompanyAsync(user.Id, defaultcompany, null).GetAwaiter().GetResult();
                    return changeres ? 1 : -1;
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ValidateUserOnLogin on error");
                return -1;
            }
            return 0;
        }

    }
}
