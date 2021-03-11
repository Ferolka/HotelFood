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
{using System.Threading.Tasks;
    public class UserDayOrderedComponent: ViewComponent
    {
        private readonly IUserDayDishesRepository _udaydishrepo;
        private readonly UserManager<HotelUser> _userManager;
        public UserDayOrderedComponent( IUserDayDishesRepository udaydishrepo, UserManager<HotelUser> userManager)
        {
            _udaydishrepo = udaydishrepo;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(DateTime daydate)
        {

            //  daydate = DateTime.Now;
            //var cid = this.User.GetCompanyID();
            //return View(_daydishrepo.DishesPerDay(daydate).ToList());
            bool allow = (_udaydishrepo.IsAllowDayEdit(daydate, this.User.GetHotelID()) && _udaydishrepo.GetConfrimedAdmin(this.User.GetUserId()));
            ViewData["AllowDelete"] = allow;
            ViewData["AllowEditDish"] = allow;
            if ((_udaydishrepo.GetCompanyOrderType(this.User.GetHotelID()) & OrderTypeEnum.OneComplexType) > 0)
            {
                ViewData["AllowEdit"] = false;
            
            }
            else
            {
                ViewData["AllowEdit"] = allow;
                
            }
            return await Task.FromResult((IViewComponentResult)View("OrderedComplexsAndDishes", _udaydishrepo.UserOrderedDay(daydate, this.User.GetUserId(), this.User.GetHotelID())));
           // return await Task.FromResult((IViewComponentResult)View("OrderedComplexs", _udaydishrepo.OrderedComplexDay(daydate, this.User.GetUserId(), this.User.GetCompanyID())));
        }
    }
}
