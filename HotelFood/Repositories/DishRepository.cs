using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Transactions;
using Microsoft.Extensions.Caching.Memory;
using HotelFood.Core;

namespace HotelFood.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelUser> _logger;
        SharedViewLocalizer _localizer;
        private readonly IMemoryCache _cache;
        public DishRepository(AppDbContext context,  ILogger<HotelUser> logger, SharedViewLocalizer localizer, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
            _cache = cache;
        }

       // public AppDbContext Context => _context;
        public async Task<bool> UpdateDishIngredients(Dish dish,int companyid)
        {
            return true;
        }

        public async Task<bool> UpdateDishEntity(Dish dish, int companyid)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!await dish.UpdateDBCompanyDataAsync(_context, _logger, companyid))
                    return false;


               
                scope.Complete();
            }
            return true;
        }
       
       
        public async Task<Result> ValidateWeightDish(int id, int companyid,Dish dish)
        {
           
            return new Result() { Success = true };
        }
        }
}
