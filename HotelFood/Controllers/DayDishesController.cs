using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using HotelFood.Core;
using HotelFood.Helpers;

namespace HotelFood.Controllers
{
    [Authorize(Roles = "Admin,CompanyAdmin,KitchenAdmin")]
    public class DayDishesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDayDishesRepository _dayDishesRepo;
        private readonly ILogger<HotelUser> _logger;
        private readonly SharedViewLocalizer _localizer;
        public DayDishesController(AppDbContext context, IDayDishesRepository daydishrepo, ILogger<HotelUser> logger, SharedViewLocalizer localizer)
        {
            _context = context;
            _dayDishesRepo = daydishrepo;
            _logger = logger;
            _localizer = localizer;
        }

        // GET: DayDishes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DayDish.ToListAsync());
        }
        public  ViewResult EditDay(DateTime daydate)
        {
            
            Hotel comp =  _context.Hotel.Find(User.GetHotelID());
            if (comp == null)
                return View("Error");
            OrderTypeEnum orderTypes = comp.GetOrderType();
           
            ViewData["MenuType"] = orderTypes;
            //ViewData["DishKindId"] = new SelectList(GetDishesKindWithEmptyList(), "Value", "Text"); ;
            daydate = DateTime.Now;

            return View(daydate);
           
        }
        //private List<SelectListItem> GetDishesKindWithEmptyList()
        //{
        //    List<SelectListItem> disheskind = _context.DishesKind.AsNoTracking()
        //          .OrderBy(n => n.Code).Select(n =>
        //              new SelectListItem
        //              {
        //                  Value = n.Id.ToString(),
        //                  Text = n.Name
        //              }).ToList();
        //    //var empty = new SelectListItem() { Value = "", Text = _localizer["NotSpecified"] };
        //    //disheskind.Insert(0, empty);
        //    return disheskind;
        //}
        public async Task<IActionResult> EditDayPartial(DateTime daydate)
        {
            daydate = DateTime.Now;

            return PartialView(await _dayDishesRepo.DishesPerDay(daydate,this.User.GetHotelID()).ToListAsync());
        }
        [HttpPost]
        public async Task<JsonResult>  SaveDayDish(int DishId, int CategoryId,int DishKindId,DateTime daydate,bool enabled)
        {
            //daydate = DateTime.Now;
            if (ModelState.IsValid)
            {
                DayDish proto = new DayDish() { DishId = DishId, Date = daydate, CategoriesId = CategoryId};
                this.AssignCompantAttr(proto);
                DayDish exsistdd = _dayDishesRepo.SelectSingleOrDefault(proto);
               // this.AssignCompantAttr(exsistdd);
                try
                {
                    if (enabled)
                    {

                        if (exsistdd != null)
                        {
                            //something wrong
                        }
                        else
                        {
                            await _context.DayDish.AddAsync(proto);
                        }
                    }
                    else
                    {
                        _context.DayDish.Remove(exsistdd);

                    }
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Save DayDish");
                    return Json(-1);
                }
            }
            
            //return Json(DishId);
            return Json(DishId);
        }
        [HttpPost]
        public async Task<JsonResult> CopyDayDish(DateTime DateFrom, DateTime DateTo,int NumDays)
        {
            bool res = false;
            try
            {

                res= await _dayDishesRepo.CopyDayMenu(DateFrom, DateTo, NumDays, User.GetHotelID());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save DayDish");
                return Json(res);
            }
            

            return Json(res);
        }
        [HttpPost]
        public async Task<JsonResult> SaveDayComplex(int ComplexId, DateTime daydate, bool enabled)
        {
            //daydate = DateTime.Now;
            if (ModelState.IsValid)
            {
                DayComplex proto = new DayComplex() { ComplexId = ComplexId, Date = daydate };
                this.AssignCompantAttr(proto);
                DayComplex exsistdd = _dayDishesRepo.SelectComplexSingleOrDefault(proto);
                // this.AssignCompantAttr(exsistdd);
                try
                {
                    if (enabled)
                    {

                        if (exsistdd != null)
                        {
                            //something wrong
                        }
                        else
                        {
                            await _context.AddAsync(proto);
                        }
                    }
                    else
                    {
                        var ordered = _context.UserDayComplex.Where
                            (x => x.ComplexId == proto.ComplexId && x.Date == proto.Date && x.HotelId == proto.HotelId).ToList();
                        if (ordered.Count() > 0)
                        {
                            //you can't delete this complex
                            return Json(new { res = "FAIL", reason = "Deleting in db" });
                        }
                        else
                        {
                            _context.Remove(exsistdd);
                        }

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Save DayDish");
                    return Json(-1);
                }
            }

            //return Json(DishId);
            return Json(ComplexId);
        }
        // GET: DayDishes/Details/5
        public async Task<IActionResult> Details(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayDish = await _context.DayDish
                .FirstOrDefaultAsync(m => m.Date == id);
            if (dayDish == null)
            {
                return NotFound();
            }

            return View(dayDish);
        }
        public IActionResult EditDayComponent(DateTime daydate)
        {
            object paramets = new { daydate = daydate };
            return ViewComponent("DayDishComponent", paramets);
        }
        public IActionResult EditDayContent(DateTime daydate, int dishKind, OrderTypeEnum menuType)
        {
            //Company comp = _context.Companies.Find(User.GetCompanyID());
            //if (comp == null)
            //    return View("Error");
            //OrderTypeEnum orderTypes = comp.GetOrderType();
            var dayMenu = new DayMenu() { Date = daydate, MenuType= menuType };
            return PartialView(dayMenu);
        }
        // GET: DayDishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DayDishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,DishId")] DayDish dayDish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dayDish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dayDish);
        }

        // GET: DayDishes/Edit/5
        public async Task<IActionResult> Edit(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayDish = await _context.DayDish.FindAsync(id);
            if (dayDish == null)
            {
                return NotFound();
            }
            return View(dayDish);
        }

        // POST: DayDishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DateTime id, [Bind("Date,DishId")] DayDish dayDish)
        {
            if (id != dayDish.Date)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dayDish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DayDishExists(dayDish.Date))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dayDish);
        }

        // GET: DayDishes/Delete/5
        public async Task<IActionResult> Delete(DateTime? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dayDish = await _context.DayDish
                .FirstOrDefaultAsync(m => m.Date == id);
            if (dayDish == null)
            {
                return NotFound();
            }

            return View(dayDish);
        }

        // POST: DayDishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DateTime id)
        {
            var dayDish = await _context.DayDish.FindAsync(id);
            _context.DayDish.Remove(dayDish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DayDishExists(DateTime id)
        {
            return _context.DayDish.Any(e => e.Date == id);
        }
    }
}
