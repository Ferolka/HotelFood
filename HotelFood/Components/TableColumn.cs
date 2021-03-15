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
    public class TableColumn: ViewComponent
    {
        public  async Task<IViewComponentResult> InvokeAsync(string field,string displayname,QueryModel queryModel, object  param=null)
        {

            //  daydate = DateTime.Now;
            ViewData["field"] = field;
            ViewData["displayname"] = string.IsNullOrEmpty(displayname)?field:displayname;
            ViewData["selectlist"] = param;
            return await Task.FromResult((IViewComponentResult)View("Default", queryModel));
        }
    }
}
