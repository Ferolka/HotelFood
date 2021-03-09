using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using Newtonsoft.Json;
using HotelFood.Data;
using HotelFood.Repositories;
using HotelFood.Models;
using HotelFood.ViewModels;

namespace HotelFood.Controllers
{
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IServiceRepository _servicerepo;
        private readonly ICompanyUserRepository _companyuserreporepo;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly SignInManager<HotelUser> _signInManager;
        public ServiceController(AppDbContext context, IServiceRepository servicerepo, ICompanyUserRepository companyuserreporepo, IWebHostEnvironment hostingEnv, SignInManager<HotelUser> signInManager)
        {
            _context = context;
            _servicerepo = servicerepo;
            _companyuserreporepo = companyuserreporepo;
            _hostingEnv = hostingEnv;
            _signInManager = signInManager;
        }

        // GET: Service
        [Authorize(Roles = "Admin, UserAdmin, ServiceAdmin")]
        public async Task<IActionResult> Index()
        {
            //var appDbContext = _context.Dishes.Include(d => d.Category).Include(d => d.Company);
            return await Task.FromResult(View());
        }
        public async Task<IActionResult> IndexNew()
        {
            //var appDbContext = _context.Dishes.Include(d => d.Category).Include(d => d.Company);
            return await Task.FromResult(View());
        }
        public async Task<IActionResult> Test()
        {
            //var appDbContext = _context.Dishes.Include(d => d.Category).Include(d => d.Company);
            return await Task.FromResult(View());
        }
        public async Task<IActionResult> Cards()
        {
            //var appDbContext = _context.Dishes.Include(d => d.Category).Include(d => d.Company);
            return await Task.FromResult(View());
        }
        public async Task<IActionResult> CardsList([Bind("SearchCriteria,SortField,SortOrder,Page,RelationFilter")] QueryModel querymodel)
        {
            //var appDbContext = _context.Dishes.Include(d => d.Category).Include(d => d.Company);
            return PartialView(await _servicerepo.GetUserCardsAsync(querymodel));
        }
        public async Task<JsonResult> GenUserCardToken(string userId)
        {
            var token = _companyuserreporepo.GenerateNewCardToken(userId, "", false);
            return await Task.FromResult(Json(new { isSuccess = true, CardTag = token, cmd = "generate" }));
        }
        public async Task<JsonResult> GenUserCardTokenConfirm(string userId, string token)
        {

            var success = _companyuserreporepo.SaveUserCardTokenAsync(userId, token);
            return await Task.FromResult(Json(new { isSuccess = success, CardTag = token, cmd = "save" }));
        }
        public async Task<IActionResult> UserCardDetails(string token)
        {

            var card = await _servicerepo.GetUserCardAsync(token);
            //    if (card == null)
            //        return NotFound();

            return PartialView(card);
        }
        [HttpPost]
        public async Task<JsonResult> Status(ServiceRequest request)
        {
            var response = new ServiceResponse();
            return await Task.FromResult(Json(response));
        }
        [HttpPost]
        public async Task<JsonResult> RequestForDelivery(ServiceRequest request)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                var fail = ServiceResponse.GetFailResult();
                fail.ErrorMessage = "Для продовження видачі, необхідно повторно зайти в систему з відповідними правами доступу";
                return Json(fail);
            }
            var response = await _servicerepo.ProcessRequestAsync(request);
            return Json(response);
        }
        //[HttpPost]
        //public async Task<JsonResult> RequestForDeliveryNew(ServiceRequest request)
        //{
        //    if (!_signInManager.IsSignedIn(User))
        //    {
        //        var fail = ServiceResponse.GetFailResult();
        //        fail.ErrorMessage = "Для продовження видачі, необхідно повторно зайти в систему з відповідними правами доступу";
        //        return Json(fail);
        //    }
        //    var response = await _servicerepo.ProcessRequestAsync(request);
        //    return Json(response);
        //}
        [HttpPost]
        public async Task<JsonResult> GetAvailableCategories(DateTime daydate)
        {

            daydate = DateTime.Now;
            if (daydate.Ticks == 0)
            {
                daydate = DateTime.Now;
            }
            var response = await _servicerepo.GetAvailableCategories(daydate);
            return Json(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetOrdersSnapshot(int? companyid, DateTime? daydate)
        {
            return Json(await _servicerepo.GetOrdersSnapshot(companyid, daydate));
        }
      
       

        public async Task<IActionResult> ServiceHistory(string request)
        {
            ServiceRequest servrequest = new ServiceRequest();
            try
            {
                if (!string.IsNullOrEmpty(request))
                    servrequest = JsonConvert.DeserializeObject<ServiceRequest>(request);
            }
            catch {
            }
            return await Task.FromResult(View(servrequest));
        }


    }
}