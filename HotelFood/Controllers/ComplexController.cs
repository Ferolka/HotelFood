using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.Data;
using HotelFood.Core;
using HotelFood.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Localization;

namespace HotelFood.Controllers
{
    [Authorize(Roles = "Admin,CompanyAdmin,KitchenAdmin")]
    public class ComplexController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IComplexRepository _complexRepo;
        private readonly ILogger<ComplexController> _logger;
        private IConfiguration _configuration;
        private readonly SharedViewLocalizer _localizer;
        private int pageRecords = 20;
        public ComplexController(AppDbContext context, IComplexRepository complexRepo, ILogger<ComplexController> logger, IConfiguration Configuration, SharedViewLocalizer localizer)        {
            _context = context;
            _complexRepo = complexRepo;
            _logger = logger;
            _configuration = Configuration;
            _localizer = localizer;
            int.TryParse(_configuration["SQL:PageRecords"], out pageRecords);

        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Complex.WhereCompany(User.GetHotelID()).ToListAsync());
        }

        public async Task<IActionResult> ListItems([Bind("SearchCriteria,SortField,SortOrder,Page")] QueryModel querymodel)//(string searchcriteria,string sortdir,string sortfield, int? page)
        {
            //QueryModel querymodel=new QueryModel() { }

            var query = this.GetQueryList(_context.Complex.Include(d => d.Category),
                querymodel,
                d => d.Name.Contains(querymodel.SearchCriteria),
                pageRecords);


            return PartialView(await query.ToListAsync());

        }
        [HttpPost]
        public async Task<IActionResult> EditModal(int id, [Bind("Id,Name,Price,DishesQuantity,CategoriesId,DishKindId")] Complex cmp, List<DishComplex> DishComplexes)
        {
            if (id != cmp.Id)
            {

                return NotFound();
            }


            var complex_orig = await _context.Complex.Include(c => c.DishComplex).ThenInclude(d => d.Dish).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
            DishComplexes.ForEach(dc => dc.Dish = _context.Dish.SingleOrDefaultAsync(c => c.Id == dc.DishId).Result);
            ViewData["CategoriesId"] = new SelectList(_context.Categories.WhereCompany(User.GetHotelID()).ToList(), "Id", "Name", cmp.CategoriesId);
           // ViewData["DishKindId"] = new SelectList(GetDishesKindWithEmptyList(), "Value", "Text", cmp.DishKindId); ;

            if (!ModelState.IsValid)
            {
                cmp.DishesQuantity = complex_orig.DishesQuantity;
                cmp.DishComplex = complex_orig.DishComplex;
                return PartialView(cmp);
            }
            if (complex_orig != null)
            {
                var validate_error = await _complexRepo.ValidateComplexUpdate(cmp, User.GetHotelID(), DishComplexes, complex_orig.DishComplex.ToList());
                if (!validate_error.Success)
                {
                    ModelState.AddModelError("", validate_error.Error);
                    cmp.DishesQuantity = complex_orig.DishesQuantity;
                    cmp.DishComplex = complex_orig.DishComplex;
                    return PartialView(cmp);
                }
            }

            var res = await this.UpdateDBCompanyDataAsyncEx2(cmp, _logger,
                 e => { return _complexRepo.UpdateComplexEntity(e, DishComplexes, User.GetHotelID()); });

            //var res = await this.UpdateDBCompanyDataAsyncEx(cmp, _logger);
            if (!ModelState.IsValid)
            {

                cmp.DishComplex = complex_orig.DishComplex;
                return PartialView(cmp);
            }



            return res;

        }


        public async Task<IActionResult> EditModal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complex = await _context.Complex.Include(c => c.DishComplex).ThenInclude(d => d.Dish).WhereCompany(User.GetHotelID()).SingleOrDefaultAsync(c => c.Id == id);
            if (complex == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories.ToList(), "Id", "Name", complex.CategoriesId);

            //ViewData["DishKindId"] = new SelectList(GetDishesKindWithEmptyList(), "Value", "Text", complex.DishKindId); ;



            return PartialView(complex);
        }
        //private List<SelectListItem> GetDishesKindWithEmptyList()
        //{
        //    List<SelectListItem> disheskind = _context.DishesKind.AsNoTracking()
        //          .OrderBy(n => n.Name).Select(n =>
        //              new SelectListItem
        //              {
        //                  Value = n.Id.ToString(),
        //                  Text = n.Name
        //              }).ToList();
        //    var empty = new SelectListItem() { Value = "", Text = _localizer["NotSpecified"] };
        //    disheskind.Insert(0, empty);
        //    return disheskind;
        //}
        public IActionResult CreateModal()
        {

            var cat = new Complex();
            if (cat == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories.WhereCompany(User.GetHotelID()).ToList(), "Id", "Name", cat.CategoriesId);
            //ViewData["DishKindId"] = new SelectList(GetDishesKindWithEmptyList(), "Value", "Text", cat.DishKindId); ;


            return PartialView("EditModal", cat);
        }



        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complex = await _complexRepo.GetByIdAsync(id);

            //   if (categories == null)
            // {
            //      return NotFound();
            //  }

            return PartialView(complex);
        }


        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var complex = await _complexRepo.GetByIdAsync(id);
                _complexRepo.Remove(complex);
                var dishComplex = _context.DishComplex.Where(d => d.ComplexId == id);
                _context.DishComplex.RemoveRange(dishComplex);
                await _complexRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteConfirmed");
                return BadRequest();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditDishes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ComplexDishViewModel ing = new ComplexDishViewModel();
            ing.DishesIds = await _context.DishComplex.WhereCompany(User.GetHotelID()).Where(d => d.ComplexId == id).Select(d => d.DishId.ToString()).ToListAsync();
            ing.Dishes = new MultiSelectList(await _context.Dish.WhereCompany(User.GetHotelID()).OrderBy(di => di.Name)
                .Select(di => new { Value = di.Id, Text = di.Name }).ToListAsync(), "Value", "Text");
            return PartialView(ing);
        }

        public async Task<IActionResult> CreateNewCourseAsync(int complexId, int course)
        {


            //var itemline = new ItemsLine();
            //itemline.ComplexId = complexId;
            //itemline.DishCourse = course + 1;
            ViewData["courseindex"] = course;
            //// itemline.DishesIds = await _context.DishComplex.WhereCompany(User.GetCompanyID()).Where(d => d.ComplexId == complexId).Where(d => d.DishCourse == course).Select(d => d.DishId.ToString()).ToListAsync();
            //itemline.DishesIds = (await _context.DishComplex.WhereCompany(User.GetCompanyID()).Where(d => d.ComplexId == complexId).Select(d => d.DishId.ToString()).ToListAsync()).ConvertAll(int.Parse);

            //itemline.Dishes = new MultiSelectList(await _context.Dishes.WhereCompany(User.GetCompanyID()).OrderBy(di => di.Name)
            //    .Select(di => new { Value = di.Id, Text = di.Name }).ToListAsync(), "Value", "Text");
            //DishComplex> itemline = new IEnumerable<DishComplex>;


            return PartialView("ComplexLineDishes", new List<DishComplex>());
        }


    }
}
