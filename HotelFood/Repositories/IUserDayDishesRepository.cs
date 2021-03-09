using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelFood.Repositories
{
    public interface IUserDayDishesRepository
    {
        bool IsAllowDayEdit(DateTime dt, int companyid);
        IQueryable<UserDayDishViewModel> DishesPerDay(DateTime dayDate, string userId,int companyid);
        IQueryable<UserDayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime daydate, string userId, int companyid, int DishKindId);
        IQueryable<UserDayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime daydate, string userId, int companyid);
        DayDish SelectSingleOrDefault(int dishId, DateTime daydate);

        IQueryable<CustomerOrdersViewModel> CustomerOrders(DateTime daydate, int companyid);
        CustomerOrdersViewModel CustomerOrders(string UserId, DateTime daydate, int companyid);
        bool SaveDay(List<UserDayDish> daydishes, HttpContext httpcontext);
        Task<bool> SaveDayComplex(List<UserDayComplex> daycomplex, string userId, int companyId);
        Task<bool> SaveDayDishInComplex(List<UserDayDish> userDayDishes, string userId, int companyId);
        Task<bool> SaveComplexAndDishesDay(List<UserDayComplex> daycomplex, List<UserDayDish> userDayDishes, string userId, int companyId);
        Task<bool> DeleteDayComplex(UserDayComplex userDayComplex, string userId, int companyId);
        Task<bool> UpdateDayComplex(UserDayComplex userDayComplex, string userId, int companyId, int newQuantity);
        Task<bool> SaveDishesDay(List<UserDayDish> userDayDishes, string userId, int companyId);
        Task<bool> DeleteDayDish(UserDayDish userDayDish, string userId, int companyId);
        Task<bool> UpdateDayDish(UserDayDish userDayDish, string userId, int companyId, int newQuantity);
        Task<bool> AddDishesDay(List<UserDayDish> userDayDishes, string userId, int companyId);
        IQueryable<UserDayComplexViewModel> ComplexPerDay(DateTime daydate, string userId, int companyid);
        IQueryable<UserDayComplexViewModel> AvaibleComplexDay(DateTime daydate, string userId, int companyid);
        IQueryable<UserDayComplexViewModel> AvaibleComplexDayForMany(DateTime daydate, string userId, int companyid);
        IQueryable<UserDayComplexViewModel> OrderedComplexDay(DateTime daydate, string userId, int companyid);
        IQueryable<UserDayDishViewModel> OrderedDishesDay(DateTime daydate, string userId, int companyid);
        IEnumerable<UserOrderedDay> UserOrderedDay(DateTime daydate, string userId, int companyid);
        WeekReportModel WeekOrder(DateTime dayFrom, DateTime dayTo, string userId, int companyid);
        OrderTypeEnum GetCompanyOrderType(int companyid);
        bool GetConfrimedAdmin(string userid);
        bool IsBalancePositive(string userid);
        //bool CanUserOrder(string userId, int companyId, string userType,decimal amount);
        //Task<decimal> UserDayLimit(string userId, int companyId, string userType, DateTime date);
        //IEnumerable<SelectListItem> DishesKind(DateTime dateFrom, DateTime dateTo, int companyid);
        //string GetUserType(string userId);
    }
}

