using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Transactions;
using HotelFood.Core;
using Microsoft.Extensions.Caching.Memory;

namespace HotelFood.Repositories
{
    public class ComplexRepository : IComplexRepository
    {
        private readonly AppDbContext _context;
        ILogger<HotelUser> _logger;
        SharedViewLocalizer _localizer;
        private readonly IMemoryCache _cache;

        public ComplexRepository(AppDbContext context, ILogger<HotelUser> logger, SharedViewLocalizer localizer, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
            _cache = cache;
     }

        public async Task<Complex> GetByIdAsync(int? id)
        {
            return await _context.Complex.FirstOrDefaultAsync(p => p.Id == id);
        }
        public void Remove(Complex complex)
        {
            _context.Remove(complex);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateComplexDishes(Complex complex, List<string> dishes, int companyid, List<ItemsLine> dishLine)
        {
            try
            {

                List<int> ds = dishes.ConvertAll(int.Parse);
                var dishComplexes = from dl in dishLine
                                    from dlc in dl.DishesIds
                                    select new DishComplex()
                                    {
                                        HotelId = companyid,
                                        ComplexId = complex.Id,
                                        DishId = dlc,
                                        DishCourse = dl.DishCourse
                                    };
                var existing_db = await _context.DishComplex.WhereCompany(companyid).Where(di => di.ComplexId == complex.Id ).ToListAsync();
               // IEnumerable<DishComplex> newRange = null;
               // empty remove all
                    //remove items which are not in db
                    // existing_db.RemoveAll(di => !dishComplexes.Any(dc=> dc.DishId== di.DishId && dc.ComplexId==di.ComplexId && dc.DishCourse==di.DishCourse));
                    //existing_db.RemoveAll(di=>true);
                    // _context.UpdateRange(existing_db);
                    _context.RemoveRange(existing_db);
                     await _context.AddRangeAsync(dishComplexes);
                    await _context.SaveChangesAsync();
                
                /*
                // IEnumerable<DishComplex> newRange1 = null;
                foreach (var dl in dishLine)
                {
                    var existing = await _context.DishComplex.WhereCompany(companyid).Where(di => di.ComplexId == complex.Id && di.DishCourse== dl.DishCourse).ToListAsync();
                    IEnumerable<DishComplex> newRange = null;
                    if (dishLine == null || dishLine.Count() == 0)
                    {
                        existing.RemoveAll(di => true);
                    }
                    else
                    {


                        existing.RemoveAll(di => !dl.DishesIds.Contains(di.DishId));
                        //                    existing.AddRange(ing.Where(p2 =>
                        //                                    existing.All(p1 => p1.IngredientId != p2)).Select(it => new DishIngredients() { IngredientId = it, DishId = dish.Id })
                        //                                    );
                        newRange = ds.Where(p2 =>
                                          existing.All(p1 => p1.DishId != p2)).Select(it => new DishComplex() { DishId = it, ComplexId = complex.Id, CompanyId = companyid, DishCourse = dl.DishCourse });
                    }
                    _context.UpdateRange(existing);
                    if (newRange != null)
                        await _context.AddRangeAsync(newRange);

                    await _context.SaveChangesAsync();

                }

                */
                //if (dishes == null || dishes.Count() == 0)
                //{
                //    existing.RemoveAll(di => true);
                //}
                //else
                //{


                //    existing.RemoveAll(di => !ds.Contains(di.DishId));
                //    //                    existing.AddRange(ing.Where(p2 =>
                //    //                                    existing.All(p1 => p1.IngredientId != p2)).Select(it => new DishIngredients() { IngredientId = it, DishId = dish.Id })
                //    //                                    );
                //    newRange = ds.Where(p2 =>
                //                      existing.All(p1 => p1.DishId != p2)).Select(it => new DishComplex() { DishId = it, ComplexId = complex.Id, CompanyId = companyid });
                //}
                //_context.UpdateRange(existing);
                //if (newRange != null)
                //    await _context.AddRangeAsync(newRange);

                //await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateComplexDishes");
                return false;
            }
            return true;
        }
        public async Task<Result> ValidateComplexUpdate(Complex complex, int companyid, List<DishComplex> dishComplexes, List<DishComplex> origdishComplexes=default)
        {
            var company = await _cache.GetCachedCompanyAsync(_context, companyid);
            if (origdishComplexes == null)
            {
                origdishComplexes =( await _context.Complex.Include(c => c.DishComplex).ThenInclude(d => d.Dish).AsNoTracking().SingleOrDefaultAsync(c => c.Id == complex.Id)).DishComplex.ToList();

            }
            if (complex.Price <= 0)
            {
                return new Result() { Success = false, Error = _localizer.GetLocalizedString("Price must be bigger than 0") };

            }
            var deleted= origdishComplexes.Where(it => !dishComplexes.Any(n => n.DishId == it.DishId));

            //int hours = company.OrderThresholdTimeH.HasValue ? company.OrderThresholdTimeH.Value : 24 * 30;
            //DateTime daydate = DateTime.Now.AddHours(-hours);

            //var existing_dishes_inorder = await (_context.DishComplex.Where(d => d.ComplexId == complex.Id)
            //           .Where(d => _context.UserDayDish.Any(ord => ord.Date >= daydate && ord.ComplexId == complex.Id && ord.DishId == d.DishId))).ToListAsync();
            var dateNow = DateTime.Now;
            DateTime min = dateNow.AddHours(-(company.OrderLeadTimeH.Value));
            min = min.AddHours(-24);
            var existing_dishes_inorder = await (_context.DishComplex.Where(d => d.ComplexId == complex.Id)
                       .Where(d => _context.UserDayDish.Any(ord => ord.Date > min && ord.ComplexId == complex.Id && ord.DishId == d.DishId))).ToListAsync();


            var deleted_whichexists= deleted.Where(it => existing_dishes_inorder.Any(n => n.DishId == it.DishId&&n.ComplexId==it.ComplexId));
            if (deleted_whichexists.Any())
            {
                return new Result() { Success = false, Error = _localizer.GetLocalizedString("DishIsOrdered") +" "+ String.Join(",",deleted_whichexists.Select(d=>d.Dish.Name)) };
            }
            return new Result() { Success = true };

        }
        public async Task<Result> UpdateComplexDishes(Complex complex,  int companyid, List<DishComplex> dishComplexes)
        {
            Result res = new Result();
            res.Success = true;
            dishComplexes.ForEach(i => { i.HotelId = companyid;
                if(i.ComplexId != complex.Id)                 
                    i.ComplexId = complex.Id; 
            });
            try
            {
                /*
                 var time = await _context.Companies.Where(x => x.Id == companyid).ToListAsync();
                int hours = (int)time.FirstOrDefault().OrderLeadTimeH;
                //TimeSpan result = TimeSpan.FromHours(hours);
                //int days = (int)result.TotalDays;
                DateTime daydate =  DateTime.Now.AddHours(-hours);
                var ordered = await _context.UserDayDish.Where(ord => ord.Date >= daydate && ord.ComplexId == complex.Id).ToListAsync();
                ordered = ordered.Where(ord => !dishComplexes.Any(dc => dc.DishId == ord.DishId)).ToList();
                // if(ordered.Any(dc => dishComplexes.Any(ord => dc.DishId == ord.DishId)))
                if (ordered.Count() > 0)
                {
                    res.Success = false;
                    res.Error = _localizer.GetLocalizedString( "DishIsOrdered");
                    return res;
                }
                */


                var existing_db = await _context.DishComplex.Where(di => di.ComplexId == complex.Id).ToListAsync();
                _context.DishComplex.RemoveRange(existing_db);
                await _context.AddRangeAsync(dishComplexes);
 
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateComplexDishes");
                res.Success = false;
                res.Error = "DbError";
                return res;
            }
            return res;
        }
        public async Task<Result> UpdateComplexEntity(Complex complex, List<DishComplex> dishComplexes, int companyid)
        {
            Result res = new Result();
            res.Success = true;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                res.Success = await complex.UpdateDBCompanyDataAsync(_context, _logger, companyid);
                if (!res.Success)
                    return res;

                res = await UpdateComplexDishes(complex, companyid, dishComplexes);
                if (!res.Success)
                    return res;
                scope.Complete();
            }
            return res;
        }

    }
}
