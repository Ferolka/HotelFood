using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.Core;
using System.Linq.Expressions;
using HotelFood.ViewModels;

namespace HotelFood.Components
{
    public class TablePager : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string field, QueryModel queryModel)
        {

            //  daydate = DateTime.Now;
            ViewData["field"] = field;
            return await Task.FromResult((IViewComponentResult)View("Default", queryModel));
        }
    }
}
