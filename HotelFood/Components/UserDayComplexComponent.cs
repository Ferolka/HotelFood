using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.Core;


namespace HotelFood.ViewComponents
{
    using Microsoft.Extensions.Caching.Memory;
    using System.Threading.Tasks;
    public class UserDayComplexComponent: ViewComponent
    {
        private readonly IUserDayDishesRepository _udaydishrepo;
        private readonly UserManager<HotelUser> _userManager;
       
        public UserDayComplexComponent( IUserDayDishesRepository udaydishrepo, UserManager<HotelUser> userManager)
        {
            _udaydishrepo = udaydishrepo;
            _userManager = userManager;
           
        }
        
        public async Task<IViewComponentResult> InvokeAsync(DayMenu day)
        {

            //  daydate = DateTime.Now;
            //var cid = this.User.GetCompanyID();
            //return View(_daydishrepo.DishesPerDay(daydate).ToList());
            DateTime daydate = day.Date;
            ViewData["AllowEdit"] = _udaydishrepo.IsAllowDayEdit(daydate, this.User.GetHotelID()) && _udaydishrepo.GetConfrimedAdmin(this.User.GetUserId())
                /*&& _udaydishrepo.IsBalancePositive(this.User.GetUserId())*/;
            ViewData["AllowAdmin"] = _udaydishrepo.GetConfrimedAdmin(this.User.GetUserId());
            //to do check balance
           // ViewData["PositiveBalance"] = _udaydishrepo.IsBalancePositive(this.User.GetUserId());
            if ((_udaydishrepo.GetCompanyOrderType(this.User.GetHotelID()) & OrderTypeEnum.OneComplexType) >0)
            {
                var complexes =  _udaydishrepo.AvaibleComplexDay(daydate, this.User.GetUserId(), this.User.GetHotelID());
               
                complexes = complexes.OrderBy(com => com.ComplexCategoryCode);
                return await Task.FromResult((IViewComponentResult)View("OneDayComplex", complexes));
           }
            else
            {
                var complexes = _udaydishrepo.AvaibleComplexDayForMany(daydate, this.User.GetUserId(), this.User.GetHotelID());
                
                complexes = complexes.OrderBy(com => com.ComplexCategoryCode);
                return await Task.FromResult((IViewComponentResult)View("Default", complexes));
                //return await Task.FromResult((IViewComponentResult)View("Default", _udaydishrepo.ComplexPerDay(daydate, this.User.GetUserId(), this.User.GetCompanyID())));
            }
            // return await Task.FromResult((IViewComponentResult)View("Default", _udaydishrepo.AvaibleComplexDay(daydate, this.User.GetUserId(), this.User.GetCompanyID())));
        }

    }
}
