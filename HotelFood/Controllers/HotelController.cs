using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.Extensions.Logging;
using HotelFood.Core;
using Microsoft.AspNetCore.Authorization;

namespace HotelFood.Controllers
{
    [Authorize(Roles = "Admin,CompanyAdmin")]
    public class HotelController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ILogger<HotelUser> _logger;
        public HotelController(AppDbContext context, ILogger<HotelUser> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Setting()
        {
            Hotel comp = await _context.Hotel.FindAsync(User.GetHotelID());
            
           // ViewData["UserTypes"] = new SelectList(GetDishesKindWithEmptyList(), "Value", "Text", cmp.DishKindId); ;
            if (comp == null)
                return View("Error");
            return View(comp);
        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setting(Hotel model, string orderType1)
        {
            OrderTypeEnum types;
            if (orderType1 == ""|| orderType1 == null)
            {
                types = OrderTypeEnum.None;
            }
            else
            {
               types = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), orderType1);
            }
            
            if (User.GetHotelID() != model.Id)
            {
                return NotFound();
            }
            model.OrderType = (int)types;
            var origCom = _context.Hotel.Where(com => com.Id == model.Id).AsNoTracking().FirstOrDefault();
            model.Code = origCom.Code;
            model.Name = origCom.Name;
            model.Phone = origCom.Phone;
            model.Email = origCom.Email;
            model.IsDefault = origCom.IsDefault;
            if (model.Code == null) model.Code = "";
            if (model.Name == null) model.Name = "";
            if (model.City == null) model.City = "";
            if (model.Country == null) model.Country = "";
            if (model.Address1 == null) model.Address1 = "";
            if (model.Address2 == null) model.Address2 = "";
            if (model.ZipCode == null) model.ZipCode = "";
            if (model.Email == null) model.Email = "";
            if (model.Phone == null) model.Phone = "";
           
            
            if (ModelState.IsValid)
            {
               
              for(int i=0;i< Request.Form.Files.Count;i++)
                {
                    Pictures pict = _context.Pictures.SingleOrDefault(
                        p => p.Id == (Request.Form.Files[i].Name=="stamplogopicture"? model.StampPictureId: model.PictureId));
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
                    }
                    _context.Add(pict);
                    await _context.SaveChangesAsync();
                    if (Request.Form.Files[i].Name == "stamplogopicture")
                    {
                        model.StampPictureId = pict.Id;
                    }
                    else
                    {
                        model.PictureId = pict.Id;
                    }
                }
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Company setting");
                    return NotFound();

                }
                return View(model);
            }
            
            return View(model);
        }
    }
}
