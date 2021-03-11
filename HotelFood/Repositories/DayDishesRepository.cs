using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotelFood.Repositories
{
    public class DayDishesRepository : IDayDishesRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserDayDishesRepository _udaydishrepo;
        private readonly ILogger<HotelUser> _logger;
        public DayDishesRepository(AppDbContext context, IUserDayDishesRepository udaydishrepo, ILogger<HotelUser> logger)
        {
            _context = context;
            _udaydishrepo = udaydishrepo;
            _logger = logger;
    }

        public IQueryable<DayDishViewModel> DishesPerDay(DateTime daydate, int companyid)
        {
            var query = from dish in _context.Dish.Where(d => d.HotelId == companyid)
                        join dd in (from subday in _context.DayDish where subday.Date == daydate && subday.HotelId == companyid select subday) on dish.Id equals dd.DishId into proto
                        from dayd in proto.DefaultIfEmpty()

                        select new DayDishViewModel() { DishId = dish.Id, DishName = dish.Name, Date = daydate, Enabled = proto.Count() > 0/*dayd != null*/ };
            return query;
        }

        public IQueryable<DayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime daydate, int companyid)
        {
            var query1 = from dish in _context.Dish.Where(d=>d.HotelId== companyid)
                         join dd in _context.DayDish.Where(d => d.HotelId == companyid && d.Date== daydate) on dish.Id equals  dd.DishId  into Details
                         from dayd in Details.DefaultIfEmpty()
                         select new DayDishViewModel
                         {
                             DishId = dish.Id,
                             CategoryId = dish.CategoriesId,
                             PictureId=dish.PictureId,
                             DishName = dish.Name,
                             Date = daydate,
                             Enabled = dayd.Date == daydate,/*dayd != null*/
                                                            //CatId= cat.Id,
                                                            // CatName=cat.Name,
                                                            //CatCode=cat.Code
                         };
            var query2 = from cat in _context.Categories
                         select new DayDishViewModelPerGategory()
                         {
                             CategoryCode = cat.Code,
                             CategoryName = cat.Name,
                             DayDishes=from dd in query1.Where(q=>q.CategoryId==cat.Id) select new DayDishViewModel()
                             {
                                 Date=dd.Date,
                                 DishId = dd.DishId,
                                 DishName = dd.DishName,
                                 IsWeight = dd.IsWeight,
                                 PictureId = dd.PictureId,
                                 Enabled = dd.Enabled
                             }
                         };
                /* !! not more working on EF 3.0*/
                /*
                var query = from entry in
                        (
                            from dish in _context.Dishes
                            where  dish.CompanyId == companyid           
                            //join cat in _context.Categories on dish.CategoriesId equals cat.Id
                            join dd in (from subday in _context.DayDish where subday.Date == daydate && subday.CompanyId == companyid select subday ) on dish.Id equals dd.DishId into Details

                            from dayd in Details.DefaultIfEmpty()
                            select new {
                                DishId = dish.Id, 
                                CategoryID = dish.CategoriesId,
                                DishName = dish.Name, 
                                Date = daydate, 
                                Enabled = dayd.Date== daydate
                            }
                        )
                        group entry by entry.CategoryID into catgroup
                        join cat in _context.Categories on new { id = catgroup.Key, cid = companyid } equals new { id = cat.Id, cid = cat.CompanyId }
                        orderby cat.Code
                        select new DayDishViewModelPerGategory()
                        {
                            CategoryCode = cat.Code,
                            CategoryName = cat.Name,
                            DayDishes = from dentry in catgroup
                                        select new DayDishViewModel()
                                        {
                                            DishId = dentry.DishId,
                                            DishName = dentry.DishName,
                                            Date = dentry.Date,
                                            Enabled = dentry.Enabled
                                        }
                        };
            //                        group dish by dish.CategoriesId into catGroup
            //                        select new DayDishViewModelPerGategory() {CategoryCode=cat;

            
            */
            return query2;
        }
        public IQueryable<DayDishViewModelPerGategory> EnabledDishesPerDay(DateTime daydate, int companyid)
        {
            var query1 = from dish in _context.Dish.Where(d => d.HotelId == companyid)
                         join dd in _context.DayDish.Where(d => d.HotelId == companyid && d.Date == daydate) on dish.Id equals dd.DishId into Details
                         from dayd in Details.DefaultIfEmpty()
                         where dayd.Date == daydate 
                         select new DayDishViewModel
                         {
                             DishId = dish.Id,
                             CategoryId = dayd.CategoriesId,
                             PictureId = dish.PictureId,
                             //DishKindId = dayd.DishKindId,
                             DishName = dish.Name,
                             Date = daydate,
                             Enabled = dayd.Date == daydate,/*dayd != null*/
                             //CatId= cat.Id,
                             // CatName=cat.Name,
                             //CatCode=cat.Code
                         };
            var query2 = from cat in _context.Categories
                         select new DayDishViewModelPerGategory()
                         {
                             CategoryId = cat.Id,
                             CategoryCode = cat.Code,
                             CategoryName = cat.Name,
                             CategoryDate = daydate,
                             DayDishes = from dd in query1.Where(q => q.CategoryId == cat.Id)
                                         select new DayDishViewModel()
                                         {
                                             Date = dd.Date,
                                             DishId = dd.DishId,
                                             DishName = dd.DishName,
                                             IsWeight = dd.IsWeight,
                                             CategoryId=cat.Id,
                                             //DishKindId = dd.DishKindId,
                                             PictureId = dd.PictureId,
                                             Enabled = dd.Enabled
                                         }
                         };
            //var ordered = _udaydishrepo.OrderedDishesDay(daydate, userId, companyid);
            //List<DayDishViewModelPerGategory> res = new List<DayDishViewModelPerGategory>();
            //query2.ForEachAsync(c => {
            //    res.Add(c);
            //    c.DayDishes.ToList().ForEach(d => {
            //        var ordDish = ordered.FirstOrDefault(ord => ord.CategoryId == d.CategoryId && ord.DishId == d.DishId && ord.DishKindId == DishKindId);
            //        if (ordDish != null)
            //        {
            //           res.Last().DayDishes.ToList().Remove(d);
            //        }
            //    });
            //});
            query2 = query2.OrderBy(c => c.CategoryCode);
            return query2;
        }
        public DayDish SelectSingleOrDefault(int dishId, DateTime daydate)
        {
            return _context.DayDish.SingleOrDefault(dd => dd.DishId == dishId && dd.Date == daydate);
        }
        public DayDish SelectSingleOrDefault(DayDish src)
        {
            return _context.DayDish.SingleOrDefault(dd => dd.DishId == src.DishId && dd.CategoriesId == src.CategoriesId &&  dd.Date == src.Date && dd.HotelId == src.HotelId);
        }

        public DayComplex SelectComplexSingleOrDefault(int complexId, DateTime daydate)
        {
            return _context.DayComplex.SingleOrDefault(dd => dd.ComplexId == complexId && dd.Date == daydate);
        }
        public DayComplex SelectComplexSingleOrDefault(DayComplex src)
        {
            return _context.DayComplex.SingleOrDefault(dd => dd.ComplexId == src.ComplexId && dd.Date == src.Date && dd.HotelId == src.HotelId);
        }

        public IQueryable<DayComplexViewModel> ComplexDay(DateTime daydate, int companyid)
        {
            var query = from comp in _context.Complex.Include(t => t.DishComplex).ThenInclude(t => t.Dish).Where(d => d.HotelId == companyid)
                        join cat in _context.Categories.Where(d => d.HotelId == companyid) on comp.CategoriesId equals cat.Id
                        join dd in (from subday in _context.DayComplex where subday.Date == daydate && subday.HotelId == companyid select subday) on comp.Id equals dd.ComplexId into proto
                        from dayd in proto.DefaultIfEmpty()
                         
                       
                        orderby dayd.Date != daydate, cat.Code
                        select new DayComplexViewModel()
                        {
                            ComplexId = comp.Id,
                            ComplexName = comp.Name,
                            Date = daydate,
                            Enabled = dayd.Date == daydate,
                            CategoryName = cat.Name,
                            DishesString = String.Join(",", comp.DishComplex.Select(d => d.Dish.Name)),
                            ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                            join dc in _context.DishComplex.WhereCompany(companyid) on d.Id equals dc.DishId
                                            where dc.ComplexId == comp.Id
                                            orderby dc.DishCourse ascending, dc.IsDefault descending
                                            select new DayComplexDishesViewModel()
                                            {

                                                DishId = d.Id,
                                                DishName = d.Name,
                                                DishReadyWeight = d.ReadyWeight,
                                                PictureId = d.PictureId,
                                                
                                                DishDescription = d.Description,
                                                DishCourse = dc.DishCourse
                                            }
                        };
            return query;
        }
        public async Task<bool> CopyDayMenu(DateTime daydate_from, DateTime daydate_to,int days, int companyId)
        {
            try
            {
                _logger.LogInformation("CopyDayMenu {0},{1},{2},{3}", daydate_from, daydate_to, days, companyId);
                var res = await _context.Database.ExecuteSqlInterpolatedAsync($"exec CopyDayMenu {daydate_from},{daydate_to} ,{days}, {companyId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CopyDayMenu");
                return false;
            }
            return true;
        }

    }
}
