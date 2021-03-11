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
{
    using System.Threading.Tasks;
    public class DayComplexComponent : ViewComponent
    {
        private readonly IDayDishesRepository _daydishrepo;
        public DayComplexComponent(IDayDishesRepository daydishrepo)
        {
            _daydishrepo = daydishrepo;
        }

        public async Task<IViewComponentResult> InvokeAsync(DayMenu day)
        {

            var complexes = _daydishrepo.ComplexDay(day.Date, this.User.GetHotelID());
            
            //return View(_daydishrepo.DishesPerDay(daydate).ToList());
            return await Task.FromResult((IViewComponentResult)View("Default", complexes));
        }
    }
}
