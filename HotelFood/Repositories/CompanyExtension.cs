using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelFood.Core;
using HotelFood.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelFood.Repositories
{


    public class EntityWrap<TEntity> where TEntity : class
    {
        public TEntity Src { get; set; }
        private HashSet<Type> excl_list = new HashSet<Type>();

        public HashSet<Type> ExclusionList { get { return excl_list; } }

        private HashSet<Type> company_list = new HashSet<Type>();

        public HashSet<Type> CompanyList { get => company_list; }

        // private HashSet<Expression<Func<TEntity, IEnumerable<TProperty>>>> collection_list = new HashSet<Expression<Func<TEntity, IEnumerable<TProperty>>>>();
     
        private HashSet<IList> colsdeleted = new HashSet<IList>();


       public HashSet<IList> CollectionList { get => colsdeleted; }

    }
    public class ColEntry<TEntity, TProperty> 
        where TEntity : class
        where TProperty : class
    {

    }
    public static class CompanyExtension
    {
        
        public static EntityWrap<TEntity> TrackCollection<TEntity,T>(this TEntity src, IQueryable<T> origin, ICollection<T> current ) where TEntity : HotelData
            where T :HotelDataOwnId
        {
            var res = new EntityWrap<TEntity>() { Src = src };
            res.TrackCollection(origin, current);
            
           
            return res;
        }

        public static EntityWrap<TEntity> TrackCollection<TEntity,T>(this EntityWrap<TEntity> src, IEnumerable<T> origin, ICollection<T> current) where TEntity : HotelData
             where T : HotelDataOwnId
        {

            var deleted = origin.Where(o=>current.FirstOrDefault(c=>c.Id==o.Id)==null).ToList();
             src.CollectionList.Add(deleted);

            return src;
        }
        public static EntityWrap<TEntity> ExcludeTrack<TEntity>(this TEntity src, Type tp) where TEntity : HotelData
        {
            var res = new EntityWrap<TEntity>() { Src=src};
            res.ExclusionList.Add(tp);
            return res;
        }
        public static EntityWrap<TEntity> ExcludeTrack<TEntity>(this EntityWrap<TEntity> src, Type tp) where TEntity : HotelData
        {
            
            src.ExclusionList.Add(tp);
            return src;
        }
        public static EntityWrap<TEntity> IncludeCompany<TEntity>(this TEntity src, Type tp) where TEntity : HotelData
        {
            var res = new EntityWrap<TEntity>() { Src = src };
            res.CompanyList.Add(tp);
            return res;
        }
        public static EntityWrap<TEntity> IncludeCompany<TEntity>(this EntityWrap<TEntity> src, Type tp) where TEntity : HotelData
        {

            src.CompanyList.Add(tp);
            return src;
        }
        public static void AssignCompantAttr<TEntity>(this Controller ctl, TEntity entity) where TEntity : HotelData
        {
            entity.HotelId = ctl.User.GetHotelID();
        }
        public static void AssignCompantAttr<TEntity>(this TEntity entity,int companyId) where TEntity : HotelData
        {
            entity.HotelId = companyId;
        }
        public static async Task<IActionResult> UpdateCompanyDataAsync<TEntity, TController>(this TController ctl, EntityWrap<TEntity> wrap, AppDbContext _context, ILogger<TController> _logger, Func<TEntity,Task<bool>>   postSaveAction = null) where TEntity : HotelDataOwnId where TController:Controller
        {
            return await ctl.UpdateCompanyDataAsync(wrap.Src, _context, _logger, postSaveAction, wrap);
        }
        public static async Task<IActionResult> UpdateCompanyDataAsync<TEntity,TController>(this TController ctl, TEntity entity, AppDbContext _context, ILogger<TController> _logger, Func<TEntity, Task<bool>> postSaveAction = null, EntityWrap<TEntity> wrap = null) where TEntity : HotelDataOwnId where TController : Controller
        {
            if (!ctl.ModelState.IsValid)
                return ctl.PartialView(entity);
            bool res = await ctl.UpdateDBCompanyDataAsync(entity, _context, _logger, wrap);
            if (!res)
                return ctl.NotFound();
            if (postSaveAction != null)
            {
                if (!await postSaveAction(entity))
                {
                    return ctl.BadRequest();
                }
            }
            return ctl.UpdateOk();
        }
        public static async Task<IActionResult> UpdateDBCompanyDataAsyncEx<TEntity,TController>(this TController ctl, TEntity entity,  ILogger<TController> _logger, Func<TEntity, Task<bool>> funcUpdate = null,EntityWrap < TEntity> wrap = null) where TEntity : HotelDataOwnId where TController: Controller
        {
            if (!ctl.ModelState.IsValid)
                return ctl.PartialView(entity);
            if(funcUpdate!=null)
                if (!await funcUpdate(entity))
                     return ctl.BadRequest();


            return ctl.UpdateOk();

        }
        public static async Task<IActionResult> UpdateDBCompanyDataAsyncEx2<TEntity,TController>(this TController ctl, TEntity entity, ILogger<TController> _logger, Func<TEntity, Task<Result>> funcUpdate = null, EntityWrap<TEntity> wrap = null) where TEntity : HotelDataOwnId where TController : Controller
        {
            if (!ctl.ModelState.IsValid)
                return ctl.PartialView(entity);
            if (funcUpdate != null)
            {
                var update_result = await funcUpdate(entity);
                if (!update_result.Success)
                    ctl.ModelState.AddModelError("general", update_result.Error);
                    return ctl.PartialView(entity);
            }

            return ctl.UpdateOk();

        }
        public static IActionResult UpdateOk( this Controller ctl)
        {
            return ctl.Json(new { res = "OK" });
        }
        public static IActionResult ErrorResult(this Controller ctl,Result res)
        {
            return ctl.Json(new { res = "FAIL",reason=res.Error });
        }
        public static async Task<bool> UpdateDBCompanyDataAsync<TEntity,TController>(this TController ctl, TEntity entity, AppDbContext _context, ILogger<TController> _logger, EntityWrap<TEntity> wrap = null) where TEntity : HotelDataOwnId where TController:Controller
        {
            try
            {
                if (wrap != null)
                    ctl.PreApplyWraps(entity, _context, wrap);
                ctl.AssignCompantAttr(entity);
                _context.Update(entity);
                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Modified)
                {
                    if(entry.OriginalValues.GetValue<int>("CompanyId")!= ctl.User.GetHotelID())  //something wrong with hack
                    {
                        throw new Exception("Fobidden");
                    }
                }
                if (wrap != null)
                    ctl.PostApplyWraps(entity, _context, wrap);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exdb)
            {
                _logger.LogError(exdb, "Update {0}", entity.GetType().Name);
                if (_context.Find(entity.GetType(), entity.Id) == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update {0}", entity.GetType().Name);
                return false; //to do
            }
            return true;
        }
        public static async Task<bool> UpdateDBCompanyDataAsync<TEntity>(this  TEntity entity, AppDbContext _context, ILogger<HotelUser> _logger,int companyId, EntityWrap<TEntity> wrap = null) where TEntity : HotelDataOwnId
        {
            try
            {
                if (wrap != null)
                    entity.PreApplyWraps( _context, wrap);
                entity.AssignCompantAttr(companyId);
                _context.Update(entity);
                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Modified)
                {
                    if (entry.OriginalValues.GetValue<int>("CompanyId") != companyId)  //something wrong with hack
                    {
                        throw new Exception("Fobidden");
                    }
                }
                if (wrap != null)
                    entity.PostApplyWraps( _context, wrap, companyId);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException exdb)
            {
                _logger.LogError(exdb, "Update {0}", entity.GetType().Name);
                if (_context.Find(entity.GetType(), entity.Id) == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update {0}", entity.GetType().Name);
                return false; //to do
            }
            return true;
        }
        private static void PreApplyWraps<TEntity>(this Controller ctl, TEntity entity, AppDbContext _context, EntityWrap<TEntity> wrap) where TEntity : HotelDataOwnId
        {
            entity.PreApplyWraps(_context, wrap);

        }
        private static void PreApplyWraps<TEntity>(this  TEntity entity, AppDbContext _context, EntityWrap<TEntity> wrap) where TEntity : HotelDataOwnId
        {
            foreach (var dels in wrap.CollectionList)
                entity.TrackCollection(_context,  dels);
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Deleted)
                    entry.State = EntityState.Detached;
            }
        }
        private static void PostApplyWraps<TEntity>(this Controller ctl, TEntity entity, AppDbContext _context, EntityWrap<TEntity> wrap) where TEntity : HotelDataOwnId
        {
            // return;
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (wrap.ExclusionList.Contains(entry.Entity.GetType()))
                {
                    entry.State = EntityState.Unchanged;

                    //_context.Entry(entry.Entity).State = EntityState.Detached;
                }
                if (wrap.CompanyList.Contains(entry.Entity.GetType()))
                {
                    ctl.AssignCompantAttr(entry.Entity as HotelData);
                }
                
            }

        }
        private static void PostApplyWraps<TEntity>(this TEntity entity, AppDbContext _context, EntityWrap<TEntity> wrap,int companyId) where TEntity : HotelDataOwnId
        {
            // return;
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (wrap.ExclusionList.Contains(entry.Entity.GetType()))
                {
                    entry.State = EntityState.Unchanged;

                    //_context.Entry(entry.Entity).State = EntityState.Detached;
                }
                if (wrap.CompanyList.Contains(entry.Entity.GetType()))
                {
                   (entry.Entity as HotelData).AssignCompantAttr(companyId);
                }

            }

        }
        private static void TrackCollection<TEntity>(this Controller ctl, AppDbContext _context, TEntity entity, IList deletedcol) where TEntity : HotelDataOwnId
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (deletedcol.Contains(entry.Entity))
                {
                    entry.State = EntityState.Deleted;
                    
                }
                
            }
        }
        private static void TrackCollection<TEntity>(this TEntity entity, AppDbContext _context, IList deletedcol) where TEntity : HotelDataOwnId
        {

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                if (deletedcol.Contains(entry.Entity))
                {
                    entry.State = EntityState.Deleted;

                }

            }
        }
    }
}
