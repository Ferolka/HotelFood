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
    public class DishController : GeneralController<Dish>
    {
        public DishController(AppDbContext context, IGenericModelRepository<Dish> repo, ILogger<Dish> logger, IConfiguration Configuration)
             : base(context, repo, logger, Configuration)
        {

        }
    }
}
