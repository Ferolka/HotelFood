using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.Core;

namespace HotelFood.ViewComponents
{using System.Threading.Tasks;
    public class DayDishComponent: ViewComponent
    {
        private readonly IDayDishesRepository _daydishrepo;
        public DayDishComponent( IDayDishesRepository daydishrepo)
        {
            _daydishrepo = daydishrepo;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(DayMenu day)
        {

            var dayDishes = _daydishrepo.EnabledDishesPerDay(day.Date, this.User.GetHotelID());
            
            return await Task.FromResult((IViewComponentResult)View("CustomDishPerCategories", dayDishes));
            //return await Task.FromResult((IViewComponentResult)View("Default", _daydishrepo.CategorizedDishesPerDay(day.Date,this.User.GetCompanyID())));
        }
    }
}
