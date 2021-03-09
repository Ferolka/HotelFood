using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using HotelFood.Models;
using HotelFood.Core;
using HotelFood.Data;

namespace HotelFood.Repositories
{
    public static class UserExtension
    {
        public static async  Task<HotelUser> FindByCardTokenAsync(this UserManager<HotelUser> src,string cardtoken)
        {
            return await src.Users.FirstOrDefaultAsync(u => u.CardTag == cardtoken);
          
        }
        public static void AssignUserAttr<TEntity>(this Controller ctl, TEntity entity) where TEntity : UserData
        {
            //entity.CompanyId = ctl.User.GetCompanyID();
            //entity.UserId = ctl.User.GetUserId();
            ctl.User.AssignUserAttr(entity);
        }
        public static void AssignUserAttr<TEntity>(this ClaimsPrincipal src , TEntity entity) where TEntity : UserData
        {
            entity.HotelId = src.GetHotelID();
            entity.UserId = src.GetUserId();
        }
        public static async Task<IActionResult> UpdateUserDataAsync<TEntity>(this Controller ctl, TEntity entity, AppDbContext _context, ILogger<HotelUser> _logger) where TEntity : UserData
        {
            if (!ctl.ModelState.IsValid)
                return ctl.PartialView(entity);
            bool res = await ctl.UpdateDBUserDataAsync(entity, _context, _logger);
            if (!res)
                return ctl.NotFound();
            return ctl.Json(new { res = "OK" });

        }
        public static async Task<bool> UpdateDBUserDataAsync<TEntity>(this Controller ctl, TEntity entity, AppDbContext _context, ILogger<HotelUser> _logger) where TEntity : UserData
        {
            try
            {
                ctl.AssignUserAttr(entity);
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exdb)
            {
                _logger.LogError(exdb, "Update {0}", entity.GetType().Name);
              //  if (_context.Find(entity.GetType(), entity.Id) == null)
              //  {
              //      return false;
              //  }
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update {0}", entity.GetType().Name);
                return false; //to do
            }
            return true;
        }
    }
}
