using HotelFood.Core;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Repositories;
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HotelFood.Controllers
{
    public class DishController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DishController> _logger;
        private IConfiguration _configuration;
        private readonly IDishRepository _dishesRepo;
        private readonly SharedViewLocalizer _localizer;
        private int pageRecords = 20;

        public DishController(AppDbContext context,
            IDishRepository dishesRepo,
            ILogger<DishController> logger,
            IConfiguration Configuration,
            SharedViewLocalizer localizer)
        {
            _context = context;
            _logger = logger;
            _configuration = Configuration;
            _dishesRepo = dishesRepo;
            _localizer = localizer;
            int.TryParse(_configuration["SQL:PageRecords"], out pageRecords);
        }

        // GET: Dishes
        public IActionResult Index()
        {
            ViewData["QueryModel"] = new QueryModel() { SortField = "Name" };
            return View(new List<Dish>());// await _context.Dishes.WhereCompany(User.GetCompanyID()).Include(d=>d.DishIngredients).Include(d=>d.DishIngredients).ThenInclude(di=>di.Ingredient).ToListAsync());
        }
        public async Task<IActionResult> ListItems([Bind("SearchCriteria,SortField,SortOrder,Page,RelationFilter")] QueryModel querymodel)
        {
            //_logger.LogWarning("Dish Controllers  - ListItems User.GetCompanyID() {0} ", User.GetCompanyID());
            //_logger.LogWarning("ListItems pageRecords {0} ", pageRecords);

            ViewData["QueryModel"] = querymodel;
            ViewData["CategoriesId"] = new SelectList(_context.Categories/*.WhereCompany(User.GetCompanyID())*/.ToList(), "Id", "Name", querymodel.RelationFilter);
            var query1 = (IQueryable<Dish>)_context.Dish/*.WhereCompany(User.GetCompanyID())*/.Include(d => d.Category);
            var query = this.GetQueryList(_context.Dish.Include(d => d.Category),
                    querymodel,
                        d => string.IsNullOrEmpty(querymodel.SearchCriteria) || d.Name.Contains(querymodel.SearchCriteria) || d.Description.Contains(querymodel.SearchCriteria),
                     pageRecords);
            if (querymodel.RelationFilter > 0)
            {
                query = query.Where(d => d.CategoriesId == querymodel.RelationFilter);
            }

            return PartialView(await query.ToListAsync());

        }
        public async Task<IActionResult> SearchView([Bind("SearchCriteria,SortField,SortOrder,Page,RelationFilter")] QueryModel querymodel)
        {

            var query = (IQueryable<Dish>)_context.Dish.Where(d => d.HotelId == User.GetHotelID()).Include(d => d.Category);

            if (querymodel != null && !string.IsNullOrEmpty(querymodel.SearchCriteria))
                query = query.Where(d => d.Name.Contains(querymodel.SearchCriteria) || d.Description.Contains(querymodel.SearchCriteria));
            query = query.Take(pageRecords);
            return PartialView(await query.ToListAsync());

        }
        

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }
        //GET: Dishes/Info/5
        //info about one dish for line 
        public async Task<IActionResult> Info(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return PartialView("DishInLine", dish);
        }
        public async Task<IActionResult> InfoDayDish(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return PartialView("DayDishLine", dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await Task.FromResult(NotFound());
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,Price,Description,CategoriesId")] Dish dish)
        {

            return await Task.FromResult(NotFound());
        }

        //     public async Task<IActionResult> GetDishPicture(int id)
        // {
        // var dish = await _context.Dishes.SingleOrDefaultAsync(d=>d.Id==id && d.CompanyId== this.User.GetCompanyID());
        //  if (dish == null || dish.DishPicture==null || dish.DishPicture.Length == 0)
        //      return File(new byte[0], "image/jpeg"); ;
        //  return File(dish.DishPicture, "image/jpeg");
        //    }
        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, /*[Bind("Id,Code,Name,Price,Description,CategoriesId,PictureId,ReadyWeight,CookingTechnologie")]*/ Dish dish /*List<string> IngredientsIds,*/)
        {
            if (id != dish.Id)
            {
                return NotFound();
            }
            if (dish.Code == null) dish.Code = "";
            if (dish.Description == null) dish.Description = "";
            if (dish.CookingTechnologie == null) dish.CookingTechnologie = "";

            var dish_orig = await _context.Dish.AsNoTracking().Where(d => d.Id == id).FirstOrDefaultAsync();
            //_context.Entry(dish_orig).Collection(d => d.DishIngredients).Query().Include(d => d.Ingredient).Load();

            if (Request.Form.Files.Count > 0)
            {
                Pictures pict = _context.Pictures.SingleOrDefault(p => p.Id == dish.PictureId);
                if (pict == null || true) //to do always new
                {
                    pict = new Pictures();
                }
                var file = Request.Form.Files[0];
                using (var stream = Request.Form.Files[0].OpenReadStream())
                {
                    byte[] imgdata = new byte[stream.Length];
                    stream.Read(imgdata, 0, (int)stream.Length);
                    pict.PictureData = imgdata;

                    PicturesController.CompressPicture(pict, 350, 350);

                    //pict.PictureData = imgdata;
                }
                _context.Add(pict);
                await _context.SaveChangesAsync();
                dish.PictureId = pict.Id;
            }


            ViewData["CategoriesId"] = new SelectList(_context.Categories.WhereCompany(User.GetHotelID()).ToList(), "Id", "Name", dish.CategoriesId);
           
            var res = await this.UpdateDBCompanyDataAsyncEx(dish, _logger,
                e => { return _dishesRepo.UpdateDishEntity(e, User.GetHotelID()); });

            if (!ModelState.IsValid)
            {
                if (ModelState["Name"].Errors.Count > 0)
                {
                    ModelState["Name"].Errors.Clear();
                    ModelState["Name"].Errors.Add(_localizer["Incorrect data"]);
                }
                if (ModelState["Code"].Errors.Count > 0)
                {
                    ModelState["Code"].Errors.Clear();
                    ModelState["Code"].Errors.Add(_localizer["Incorrect data"]);
                }
                if (ModelState["Price"].Errors.Count > 0)
                {
                    ModelState["Price"].Errors.Clear();
                    ModelState["Price"].Errors.Add(_localizer["Incorrect data"]);
                }
                if (ModelState["ReadyWeight"].Errors.Count > 0)
                {
                    ModelState["ReadyWeight"].Errors.Clear();
                    ModelState["ReadyWeight"].Errors.Add(_localizer["Incorrect data"]);
                }
                if (ModelState["Description"].Errors.Count > 0)
                {
                    ModelState["Description"].Errors.Clear();
                    ModelState["Description"].Errors.Add(_localizer["Incorrect data"]);
                }


               
                return PartialView(dish);
            }

            return res;
        }


       
        public async Task<IActionResult> EditModal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish.FindAsync(id);
            if (dish == null || dish.HotelId != User.GetHotelID())
            {
                return NotFound();
            }
         

            ViewData["CategoriesId"] = new SelectList(_context.Categories.WhereCompany(User.GetHotelID()).ToList(), "Id", "Name", dish.CategoriesId);
            return PartialView(dish);
        }

       
        
        [HttpGet]
        public ActionResult Search(string term, bool isShort = true)
        {
            var result = _context.Dish.Where(d => d.HotelId == User.GetHotelID()).Where(d => d.Name.Contains(term));
            if (isShort)
            {
                return Ok(result.Select(d => new { Id = d.Id, Name = d.Name, Price = d.Price }));
            }

            return Ok(result);


        }
        public IActionResult CreateModal()
        {

            var dish = new Dish();
            if (dish == null)
            {
                return NotFound();
            }
            dish.Code = _context.Categories.Count().ToString();
            ViewData["CategoriesId"] = new SelectList(_context.Categories.ToList(), "Id", "Name", dish.CategoriesId);
            return PartialView("EditModal", dish);
        }
        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return PartialView(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dish = await _context.Dish.FindAsync(id);
                _context.Dish.Remove(dish);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbex)
            {
                _logger.LogError(dbex, "Delete confirmed error");
                return StatusCode((int)HttpStatusCode.FailedDependency);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteConfirmed");
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
        
        private bool DishExists(int id)
        {
            return _context.Dish.Any(e => e.Id == id);
        }
    }
}
