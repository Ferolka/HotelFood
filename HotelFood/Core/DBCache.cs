
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;

namespace HotelFood.Core
{
    public static class DBCache
    {
        public static async Task<List<Hotel>> GetCachedCompaniesAsync(this IMemoryCache cache,AppDbContext context)
        {
            var res=await cache.GetOrCreateAsync("CompanyList", async entry =>
            {

                entry.SetPriority(CacheItemPriority.Normal);
               // entry.AddExpirationToken(changeToken);
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(100);

                return await context.Hotel.AsNoTracking().ToListAsync();
            });
            return  res;
        }
        public static async Task<Hotel> GetCachedCompanyAsync(this IMemoryCache cache, AppDbContext context,int companyid)
        {
            return (await cache.GetCachedCompaniesAsync(context)).FirstOrDefault(c => c.Id == companyid);
        }
    }
}
