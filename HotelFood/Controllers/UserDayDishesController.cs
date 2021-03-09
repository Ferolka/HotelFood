using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HotelFood.Repositories;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.Core;
using Microsoft.Extensions.Logging;
using HotelFood.ViewModels;

namespace HotelFood.Controllers
{
    [Authorize]
    public class UserDayDishesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserDayDishesRepository _userdaydishesrepo;
        private readonly UserManager<HotelUser> _userManager;
        private readonly ILogger<HotelUser> _logger;
       // private readonly IEmailService _email;
        //private readonly IInvoiceRepository _invoicerepo;
        private readonly SharedViewLocalizer _localizer;
        //private readonly IUserDayDishesRepository _udaydishrepo;

        public UserDayDishesController(AppDbContext context, 
            IUserDayDishesRepository ud, UserManager<HotelUser> um, 
            ILogger<HotelUser> logger,
            //IEmailService email, IInvoiceRepository invoicerepo,
            SharedViewLocalizer localizer)
        {
            _context = context;
            _userManager = um;
            _userdaydishesrepo = ud;
            _logger = logger;
            //_email = email;
            //_invoicerepo = invoicerepo;
            _localizer = localizer;
            // _udaydishrepo = udaydishrepo;
        }

        // GET: UserDayDishes
   
        [Route("UserDayDishes")]
        [Route("MyOrders")]
        public async Task<IActionResult> Index()
        {
            var user=await  _userManager.GetUserAsync(HttpContext.User);
            if(user==null)
                return NotFound();
            UserDayEditModel model = new UserDayEditModel()
            {
                DayDate = DateTime.Now,
                //ShowComplex = user.MenuType.HasValue && (user.MenuType.Value & 1) > 0,
                //ShowDishes = user.MenuType.HasValue && (user.MenuType.Value & 2) > 0,
                ShowComplex = (_userdaydishesrepo.GetCompanyOrderType(this.User.GetHotelID()) & (OrderTypeEnum.OneComplexType | OrderTypeEnum.Complex)) > 0,
                ShowDishes = (_userdaydishesrepo.GetCompanyOrderType(this.User.GetHotelID()) & OrderTypeEnum.Dishes) > 0

            };
            DateTime daydate = DateTime.Now;
            //daydate = daydate.AddDays(1);
            if (daydate.DayOfWeek == DayOfWeek.Saturday|| daydate.DayOfWeek == DayOfWeek.Sunday)
            {
                daydate = daydate.AddDays(2);
            }
            DateTime startDate = daydate.StartOfWeek(DayOfWeek.Monday);
            DateTime endDate = startDate.AddDays(6);
            //var list = _userdaydishesrepo.DishesKind(startDate, endDate, User.GetCompanyID());
            //ViewData["DishKindId"] = new SelectList(list, "Value", "Text", list.FirstOrDefault());
            return View(model); //await _userdishes.CategorizedDishesPerDay(DateTime.Now, _userManager.GetUserId(HttpContext.User)).ToListAsync());
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
        //    //disheskind.FirstOrDefault().Selected = true;
        //    //var empty = new SelectListItem() { Value = "", Text = _localizer["All"] };
        //   // disheskind.Insert(0, empty);
        //    return disheskind;
        //}
        public async Task<IActionResult>  EditUserDay(DateTime daydate, int dishKind)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return NotFound();
            UserDayEditModel model = new UserDayEditModel()
            {
                DayDate = daydate,
                DayMenu =new DayMenu() { Date = daydate, DishKind = dishKind },
                ShowComplex = (_userdaydishesrepo.GetCompanyOrderType(this.User.GetHotelID()) & (OrderTypeEnum.OneComplexType | OrderTypeEnum.Complex) ) >0,
                //ShowComplex = user.MenuType.HasValue && (user.MenuType.Value & 1) > 0,
                ShowDishes = (_userdaydishesrepo.GetCompanyOrderType(this.User.GetHotelID()) & OrderTypeEnum.Dishes ) > 0
            };
            return PartialView(model);
        }
        //public async Task<IActionResult> GetDishesKind(DateTime daydate, int dishKind)
        //{
        //    //if (daydate.DayOfWeek == DayOfWeek.Saturday || daydate.DayOfWeek == DayOfWeek.Sunday)
        //    //{
        //    //    daydate = daydate.AddDays(2);
        //    //}
        //    DateTime startDate = daydate.StartOfWeek(DayOfWeek.Monday);
        //    DateTime endDate = startDate.AddDays(6);
        //    var list = _userdaydishesrepo.DishesKind(startDate, endDate,User.GetCompanyID());
        //    var selected = list.Where(sl => sl.Value == dishKind.ToString()).FirstOrDefault();
        //    if (selected == null)
        //    {
        //        selected = list.FirstOrDefault();
        //    }
        //    ViewData["DishKindId"] = new SelectList(list, "Value", "Text", selected);
        //    return PartialView("DishKinds");
        //}
        // GET: UserDayDishes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDayDish = await _context.UserDayDish
                .Include(u => u.Dish)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userDayDish == null)
            {
                return NotFound();
            }

            return View(userDayDish);
        }

        // GET: UserDayDishes/Create
        public IActionResult Create()
        {
            ViewData["DishId"] = new SelectList(_context.Dish, "Id", "Code");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserDayDishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,DishId,Quantity,UserId")] UserDayDish userDayDish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userDayDish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DishId"] = new SelectList(_context.Dish, "Id", "Code", userDayDish.DishId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userDayDish.UserId);
            return View(userDayDish);
        }

        // GET: UserDayDishes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDayDish = await _context.UserDayDish.FindAsync(id);
            if (userDayDish == null)
            {
                return NotFound();
            }
            ViewData["DishId"] = new SelectList(_context.Dish, "Id", "Code", userDayDish.DishId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userDayDish.UserId);
            return View(userDayDish);
        }
        [HttpPost]
        public async Task<JsonResult> SaveDay(List<UserDayDish> daydishes)
        {
            //await  _email.SendEmailAsync("yurik.kovalenko@gmail.com", "catering", "new order");
            DateTime daydate = DateTime.Now;
            if (daydishes.Count > 0)
                daydate = daydishes.First().Date;
            else
            {
                return await Task.FromResult(Json(new { res = "FAIL",reason="Empty" }));
            }
            if(!_userdaydishesrepo.IsAllowDayEdit(daydate, User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "OutDate" }));
            }
           // await _email.SendInvoice(User.GetUserId(), daydate, User.GetHotelID());
            if (  _userdaydishesrepo.SaveDay(daydishes, this.HttpContext)){
                return await Task.FromResult(Json(new { res = "OK" }));
            }
            else
            {
                return await Task.FromResult(Json(new { res = "FAIL",reason="Error" }));
            }
            /*
            try
            {
                daydishes.ForEach(d =>
                {
                    //await saveday(d);
                    this.AssignUserAttr(d);
                    var userDayDish = _context.UserDayDish.Find(d.UserId, d.Date, d.DishId,d.CompanyId  );
                    if (userDayDish != null)
                    {
                        userDayDish.Quantity = d.Quantity;
                        userDayDish.Price = d.Price;
                        _context.Update(userDayDish);
                    }
                    else if(d.Quantity>0)
                    {
                        //d.UserId = this.User.GetUserId();
                       
                        _context.Add(d);
                    }

                });
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Update user day dish");
                return await Task.FromResult(Json(new { res = "FAIL" }));
            }
            return await Task.FromResult(Json(new { res = "OK" }));
            */
        }
        public async Task<JsonResult> SaveDayComplex(List<UserDayComplex> UserDayComplex, List<UserDayDish> UserDayDish)
        {
            
            try
            {
                
                var daycomplexes = UserDayComplex;
                var duplicateKeys = daycomplexes.GroupBy(x => x)
                            .Where(group => group.Count() > 1)
                            .Select(group => group.Key);
                //await  _email.SendEmailAsync("yurik.kovalenko@gmail.com", "catering", "new order");
                DateTime daydate = DateTime.Now;
                //string userType = _userdaydishesrepo.GetUserType(User.GetUserId()) + "Limit";
                decimal total = 0;
                foreach(var usc in UserDayComplex)
                {
                    total += usc.Quantity * usc.Price;
                }
                //if (!_userdaydishesrepo.CanUserOrder(User.GetUserId(), User.GetCompanyID(), userType, total))
                //{
                //    return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Not enough money")));
                //}

                if (daycomplexes.Count > 0)
                    daydate = daycomplexes.First().Date;
                else
                {
                    return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Empty")));
                }
               

                if (!_userdaydishesrepo.IsAllowDayEdit(daydate, User.GetHotelID()))
                {
                    return await Task.FromResult(Json(JSONResultResponse.GetFailResult("OutDate")));
                }
                //var res = _userdaydishesrepo.OrderedComplexDay(daydate, User.GetUserId(), User.GetCompanyID()).ToList();
                //bool ordered = res.Any(x => daycomplexes.Any(y => y.ComplexId == x.ComplexId));
                if (duplicateKeys.Count() != 0 /*|| ordered*/)
                {
                    //if (duplicateKeys.Count() != 0)
                    //{
                        _logger.LogWarning("Duplicates from front in User Day {0} userId {1}", daydate, User.GetUserId());
                    //}
                    //else
                    //{
                    //    _logger.LogWarning("Already ordered complex in User Day {0} userId {1}", daydate, User.GetUserId());
                    //}
                    return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
                }


                if (await _userdaydishesrepo.SaveComplexAndDishesDay(daycomplexes, UserDayDish, User.GetUserId(), User.GetHotelID()))
                {
                    //await _email.SendInvoice(User.GetUserId(), daydate, User.GetCompanyID());
                    return await Task.FromResult(Json(JSONResultResponse.GetOKResult()));

                }
                else
                {
                    return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db" )));
                }
            } catch(Exception ex)
            {
                _logger.LogError(ex,"SaveDayComplex error");
                return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
            }
 
            
        }
        //public async Task<JsonResult> SaveDayDish(List<UserDayDish> UserDayDish)
        //{

        //    try
        //    {
        //        UserDayDish.ForEach(d => {
        //            if (d.IsWeight)
        //            {
        //                d.Quantity = (int)(d.OrderQuantity / d.Base);
        //            }
        //            else
        //            {
        //                d.Quantity = (int)d.OrderQuantity;
        //            }
        //        });
        //        //string userType = User.GetUserType().ToString() + "Limit";
        //        string userType = _userdaydishesrepo.GetUserType(User.GetUserId()) + "Limit";
        //        decimal total = 0;
        //        foreach (var usc in UserDayDish)
        //        {
        //            total += usc.Quantity * usc.Price;
        //        }
        //        //var user = await _userManager.FindByIdAsync(userid);
        //        //string userType = user.GetUserType().ToString() + "Limit";
        //        if (!_userdaydishesrepo.CanUserOrder(User.GetUserId(), User.GetCompanyID(), userType, total))
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Not enough money")));
        //        }
        //        var userDayDishes = UserDayDish.Where(d => d.Quantity > 0).ToList();
        //        if (userDayDishes.Count == 0)
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Empty")));
        //        }
        //        if (!_userdaydishesrepo.IsAllowDayEdit(UserDayDish.First().Date, User.GetCompanyID()))
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("OutDate")));
        //        }
        //        if (await _userdaydishesrepo.SaveDishesDay(UserDayDish, User.GetUserId(), User.GetCompanyID()))
        //        {
        //            //await _email.SendInvoice(User.GetUserId(), daydate, User.GetCompanyID());
        //            return await Task.FromResult(Json(JSONResultResponse.GetOKResult()));

        //        }
        //        else
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "SaveDayDish error");
        //        return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
        //    }

        //}
        //public async Task<JsonResult> SendWeekInvoice(string day)
        //{
        //    DateTime daydate = Convert.ToDateTime(day);
        //    await _email.SendWeekInvoice(User.GetUserId(), daydate, User.GetCompanyID());
        //    return await Task.FromResult(Json(JSONResultResponse.GetOKResult()));
        //}
        public async Task<IActionResult> GetWeekOrderDetails(string day)
        {
            DateTime daydate = Convert.ToDateTime(day);
            string userid = User.GetUserId();
            int comapnyid = User.GetHotelID();
            try
            {
                var test = _userdaydishesrepo.WeekOrder(daydate, daydate.AddDays(6), userid, comapnyid);
                return PartialView("WeekReport", test);
                //test = test.OrderBy(a => a.Date);
                //var model = _invoicerepo.CustomerInvoice(userid, daydate, comapnyid);

                ////var testList = test.ToList();
                ////var inList = new List<InvoiceItemModel>();
                ////testList.ForEach(a =>
                ////{
                ////    inList.Add(new InvoiceItemModel() { DayComplex = a });
                ////});
                //// model.Items = inList;

                //var avaible = _userdaydishesrepo.AvaibleComplexDay(daydate, userid, comapnyid);
                //var items = model.Items.ToList();
                //if (avaible.Count() > 0 && items.Count() == 0)
                //{
                //    var inItem = new InvoiceItemModel();
                //    inItem.DayComplex = new UserDayComplexViewModel();
                //    inItem.DayComplex.Date = daydate;
                //    items.Add(inItem);

                //}
                //model.Items = items;
                //for (int i = 0; i < 6; i++)
                //{

                //    daydate = daydate.AddDays(1);

                //    avaible = _userdaydishesrepo.AvaibleComplexDay(daydate, userid, comapnyid);

                //    var nextModel = _invoicerepo.CustomerInvoice(userid, daydate, comapnyid);
                //    var nextItems = nextModel.Items.ToList();
                //    var onlyComplex = new List<InvoiceItemModel>();
                //    foreach (var it in nextItems)
                //    {
                //        if (it.DayComplex != null)
                //        {
                //            onlyComplex.Add(it);
                //        }
                //    }
                //    nextItems = onlyComplex;
                //    items = model.Items.ToList();

                //    if (avaible.Count() > 0 && nextModel.Items.ToList().Count() == 0)
                //    {
                //        var inItem = new InvoiceItemModel();
                //        inItem.DayComplex = new UserDayComplexViewModel();
                //        inItem.DayComplex.Date = daydate;

                //        inItem.DayComplex.Enabled = false;

                //        items.Add(inItem);

                //        //items.AddRange(nextModel.Items.ToList());
                //    }
                //    items.AddRange(nextItems);

                //    model.Items = items;



                //}
                //return PartialView("~/Views/Invoice/EmailWeekInvoice.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWeekOrderDetails ");
                return await Task.FromResult(Json(new { res = "FAIL", reason = "Getting from db Email week invoice" }));
            }

                //await _email.SendWeekInvoice(User.GetUserId(), daydate, User.GetCompanyID());
                
        }
        // POST: UserDayDishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Date,DishId,Quantity,UserId")] UserDayDish userDayDish)
        {
            if (id != userDayDish.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userDayDish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDayDishExists(userDayDish.UserId))
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
            ViewData["DishId"] = new SelectList(_context.Dish, "Id", "Code", userDayDish.DishId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userDayDish.UserId);
            return View(userDayDish);
        }

        // GET: UserDayDishes/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCom(UserDayComplex UserDayComplex)
        {
            if (UserDayComplex == null)
            {
                return NotFound();
            }
            var comName = await _context.Complex.FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID()
                && x.Id == UserDayComplex.ComplexId);
            var userDayDish = await _context.UserDayComplex.
                FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID()&&x.UserId==User.GetUserId()
                && x.Date == UserDayComplex.Date && x.ComplexId == UserDayComplex.ComplexId);
            //var userDayDish1 =  _context.UserDayComplex.
            //    Where(x => x.CompanyId == User.GetCompanyID()
            //    && x.Date == UserDayComplex.Date && x.ComplexId == UserDayComplex.ComplexId);
            //var userDayDish = userDayDish1.FirstOrDefault();
            if (userDayDish == null)
            {
                return NotFound();
            }
            userDayDish.Complex.Name = comName.Name;
            //userDayDish
            return PartialView("Delete",userDayDish);
        }

        // POST: UserDayDishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userDayDish = await _context.UserDayDish.FindAsync(id);
            _context.UserDayDish.Remove(userDayDish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //delete ordered complex
        public async Task<JsonResult> DeleteDayComplex(UserDayComplex UserDayComplex)
        {
            //var daycomplexes = UserDayComplex;
            //await  _email.SendEmailAsync("yurik.kovalenko@gmail.com", "catering", "new order");
            DateTime daydate = DateTime.Now;
            //          bool res = _userdaydishesrepo.SaveDayDishInComplex(UserDayDish, this.HttpContext);
            
            if (!_userdaydishesrepo.IsAllowDayEdit(UserDayComplex.Date, User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "OutDate" }));
            }
            //await _email.SendInvoice(User.GetUserId(), daydate, User.GetCompanyID());

            if (await _userdaydishesrepo.DeleteDayComplex(UserDayComplex, User.GetUserId(), User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "OK" }));
            }
            else
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "Deleting in db" }));
            }


        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCom(UserDayComplex UserDayComplex)
        {
            if (UserDayComplex == null)
            {
                return NotFound();
            }
            var comName = await _context.Complex.FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID()
                && x.Id == UserDayComplex.ComplexId);
            var userDayDish = await _context.UserDayComplex.
                FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID() && x.UserId == User.GetUserId()
                && x.Date == UserDayComplex.Date && x.ComplexId == UserDayComplex.ComplexId);
            //var userDayDish1 =  _context.UserDayComplex.
            //    Where(x => x.CompanyId == User.GetCompanyID()
            //    && x.Date == UserDayComplex.Date && x.ComplexId == UserDayComplex.ComplexId);
            //var userDayDish = userDayDish1.FirstOrDefault();
            if (userDayDish == null)
            {
                return NotFound();
            }
            userDayDish.Complex.Name = comName.Name;
            //userDayDish
            return PartialView("EditComplex", userDayDish);
        }
        public async Task<JsonResult> EditDayComplex(UserDayComplex UserDayComplex, int newQuantity)
        {
            
            if (!_userdaydishesrepo.IsAllowDayEdit(UserDayComplex.Date, User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "OutDate" }));
            }

            if (newQuantity <= 0)
            {
                if (await _userdaydishesrepo.DeleteDayComplex(UserDayComplex, User.GetUserId(), User.GetHotelID()))
                {
                    return await Task.FromResult(Json(new { res = "OK" }));
                }
            }
            if (await _userdaydishesrepo.UpdateDayComplex(UserDayComplex, User.GetUserId(), User.GetHotelID(), newQuantity))
            {
                return await Task.FromResult(Json(new { res = "OK" }));
            }
            else
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "Deleting in db" }));
            }


        }
        public async Task<IActionResult> DeleteDish(UserDayDish UserDayDish)
        {
            if (UserDayDish == null)
            {
                return NotFound();
            }
            var dish = await _context.Dish.FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID()
                && x.Id == UserDayDish.DishId);
            var userDayDish = await _context.UserDayDish.
                FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID() && x.UserId == User.GetUserId()
                && x.Date == UserDayDish.Date && x.DishId == UserDayDish.DishId/*&&x.DishKindId==UserDayDish.DishKindId*/&&x.CategoriesId==UserDayDish.CategoriesId);
    
            if (userDayDish == null)
            {
                return NotFound();
            }
            userDayDish.Dish=dish;
            //userDayDish
            return PartialView("DeleteDish", userDayDish);
        }
        public async Task<JsonResult> DeleteDayDish(UserDayDish UserDayDish)
        {
           

            if (!_userdaydishesrepo.IsAllowDayEdit(UserDayDish.Date, User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "OutDate" }));
            }


            if (await _userdaydishesrepo.DeleteDayDish(UserDayDish, User.GetUserId(), User.GetHotelID()))
            {
                return await Task.FromResult(Json(new { res = "OK" }));
            }
            else
            {
                return await Task.FromResult(Json(new { res = "FAIL", reason = "Deleting in db" }));
            }


        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDish(UserDayDish UserDayDish)
        {
            if (UserDayDish == null)
            {
                return NotFound();
            }
            var dish = await _context.Dish.FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID()
                && x.Id == UserDayDish.DishId);
            var userDayDish = await _context.UserDayDish.
                FirstOrDefaultAsync(x => x.HotelId == User.GetHotelID() && x.UserId == User.GetUserId()
                && x.Date == UserDayDish.Date && x.DishId == UserDayDish.DishId/* && x.DishKindId == UserDayDish.DishKindId*/ && x.CategoriesId == UserDayDish.CategoriesId);

            if (userDayDish == null)
            {
                return NotFound();
            }
            userDayDish.Dish = dish;
            return PartialView("EditDish", userDayDish);
        }
        //public async Task<JsonResult> EditDayDish(UserDayDish UserDayDish, decimal newQuantity)
        //{
        //    int newQuan = 0;
        //    if (UserDayDish.IsWeight)
        //    {
        //        newQuan = (int)(newQuantity / UserDayDish.Base);
        //    }
        //    else {
        //        newQuan = (int)newQuantity;
        //            }
        //    if (!_userdaydishesrepo.IsAllowDayEdit(UserDayDish.Date, User.GetCompanyID()))
        //    {
        //        return await Task.FromResult(Json(new { res = "FAIL", reason = "OutDate" }));
        //    }

        //    if (newQuantity <= 0)
        //    {
        //        if (await _userdaydishesrepo.DeleteDayDish(UserDayDish, User.GetUserId(), User.GetCompanyID()))
        //        {
        //            return await Task.FromResult(Json(new { res = "OK" }));
        //        }
        //    }
        //    if (await _userdaydishesrepo.UpdateDayDish(UserDayDish, User.GetUserId(), User.GetCompanyID(), newQuan))
        //    {
        //        return await Task.FromResult(Json(new { res = "OK" }));
        //    }
        //    else
        //    {
        //        return await Task.FromResult(Json(new { res = "FAIL", reason = "Deleting in db" }));
        //    }


        //}
        private bool UserDayDishExists(string id)
        {
            return _context.UserDayDish.Any(e => e.UserId == id);
        }
        //public async Task<IActionResult> AddOrder(string userid, DateTime date)
        //{
        //    var dishes = _userdaydishesrepo.CategorizedDishesPerDay(date, userid, this.User.GetCompanyID()); 
        //    return PartialView(dishes);
        //}
        //[HttpPost]
        //public async Task<JsonResult> AddDayDish(List<UserDayDish> UserDayDish)
        //{

        //    try
        //    {
        //        UserDayDish.ForEach(d => {
        //            if (d.IsWeight)
        //            {
        //                d.Quantity = (int)(d.OrderQuantity / d.Base);
        //            }
        //            else
        //            {
        //                d.Quantity = (int)d.OrderQuantity;
        //            }
        //        });
        //        string userid = UserDayDish.First().UserId;
        //        var user = await _userManager.FindByIdAsync(userid);
        //        string userType = user.GetUserType().ToString() + "Limit";
        //        decimal total = 0;
        //        foreach (var usc in UserDayDish)
        //        {
        //            total += usc.Quantity * usc.Price;
        //        }
        //        if (await _userdaydishesrepo.UserDayLimit(userid, User.GetCompanyID(), userType, UserDayDish.First().Date)<total)
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Not enough money")));
        //        }
        //        var userDayDishes = UserDayDish.Where(d => d.Quantity > 0).ToList();
        //        if (userDayDishes.Count == 0)
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Empty")));
        //        }

        //        if (await _userdaydishesrepo.AddDishesDay(UserDayDish, userid, User.GetCompanyID()))
        //        {

        //            return await Task.FromResult(Json(JSONResultResponse.GetOKResult()));

        //        }
        //        else
        //        {
        //            return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
        //        }
        //        return await Task.FromResult(Json(JSONResultResponse.GetOKResult()));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "AddDayDish error");
        //        return await Task.FromResult(Json(JSONResultResponse.GetFailResult("Adding to db")));
        //    }

        //}

    }
}
