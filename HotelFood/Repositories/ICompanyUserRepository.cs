
using HotelFood.Models;
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelFood.Repositories
{
    public interface ICompanyUserRepository
    {
        string GetCurrentCompany();
        Task<List<Hotel>> GetCurrentUsersCompaniesAsync(string userId);

        Task<bool> ChangeUserCompanyAsync(string userId, int companyid, ClaimsPrincipal claims);

        Task<List<UserGroups>> GetUserGroups(int companyId);

        Task<List<UserRoleViewModel>> GetRolesForUserAsync(HotelUser user);
        Task<bool> PostUpdateUserAsync(HotelUser user, bool isNew = false);
        //Task<List<HotelUser>> GetUserChilds(string userId, int companyId,bool onlyChild= false);
        //Task<bool> PostUpdateChildUserAsync(HotelUser childuser, HotelUser parentuser);
        ////Task<AddBalanceViewModel> AddBalanceViewAsync(string userId);
        //decimal GetUserBalance();
        //Task<decimal> GetUserBalanceAsync();
        //Task<bool> AddNewUserChild(string userId, int companyId);
        Task<List<Hotel>> GetCompaniesAsync();
        Task<List<AssignedCompanyEditViewModel>> GetAssignedCompaniesEdit(string userId);
        Task<bool> AddCompaniesToUserAsync(string userid, IList<int> companiesIds);
        string GetTokenForUser(HotelUser user);
        string GenerateNewCardToken(string userid, string cardUid, bool addHash = false);
        Task<bool> SaveUserCardTokenAsync(string userId, string token);
       // Task<UserSubGroupViewModel> GetSubGroupTree(int companyId);
        Task<List<AssignedCompanyEditViewModel>> GetAssignedEditCompanies(string userId);
        Task<int> GetUserCompanyCount(string userId);
        //Task<List<int>> UserPermittedSubGroups(string userId, int companyid);
        // Task<List<UserSubGroup>> GetUserSubGroups(int companyId);
        UpdateUserModel GetUpdateUserModel(HotelUser user);
        //List<int> GetUserSubGroups(string userId, int companyid);
        //int GetUserSubGroupId(string userId);
        Task<bool> ValidateBasicAuthAsync(string val);
        Task<HotelGuests> GetCurrentUserCompaniesUserAsync(string userId);
        Task<HotelGuests> GetUserCompaniesUserAsync(string userId, int companyId);
        //int GetTopLevelSubGroup();
        //string GetUserSubGroupName(int subgroupid);
        //List<SelectListItem> GetUserSubgroupsdWithEmptyList();
        List<SelectListItem> GetCompaniesWithEmptyList();
        int ValidateUserOnLogin(HotelUser user);
    }
}
