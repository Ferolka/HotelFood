using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Controllers
{
    public class CategoriesController : GeneralController<Categories>
    {
        public CategoriesController(AppDbContext context, IGenericModelRepository<Categories> repo, ILogger<Categories> logger, IConfiguration Configuration)
             : base(context, repo, logger, Configuration)
        {

        }
    }
}
