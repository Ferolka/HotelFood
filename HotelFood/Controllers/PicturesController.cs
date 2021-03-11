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

namespace HotelFood.Controllers
{
    public class PicturesController : Controller
    {
        private readonly AppDbContext _context;
        private static Image<Rgba32> image_nophoto = null;

        
        public PicturesController(AppDbContext context)
        {
            _context = context;
        }
        public static bool CompressPicture(Pictures pict, int width,int height)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(pict.PictureData))
            {
                image.Mutate(x => x.Resize(width, height));
                using (MemoryStream ms = new MemoryStream())
                {
                    image.SaveAsJpeg(ms);
                    pict.PictureData= ms.GetBuffer();
                }
            }
            return true;
        }
        private Image<Rgba32> GetNoPhoto()
        {
            if (image_nophoto != null)
                return image_nophoto;
            string nophotofile = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\images\nophoto.jpg"}";
            image_nophoto = Image.Load<Rgba32>(nophotofile);
            return image_nophoto;
        }
        public static byte[] MutateImageToStream(Image image , int? width, int? height)
        {
            if(image!=null && width.HasValue && width.Value > 0  && height.HasValue && height.Value > 0)
            {
                image.Mutate(x => x
                            .Resize(width.Value, height.Value));
            }
            if (image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.SaveAsJpeg(ms);
                    return ms.GetBuffer();
                }
            }
            return null;
        }
        [ResponseCache( VaryByQueryKeys = new string[] { "*"/* "id","width","height"*/ }, Duration = 300*60,Location =ResponseCacheLocation.Any,NoStore =false)]
        public async Task<IActionResult> GetPicture(int? id,int? width,int? height)
        {

            if (id.HasValue)
            {
                var pict = await _context.Pictures.FindAsync(id);
                if (pict != null && pict.PictureData != null && pict.PictureData.Length != 0)
                {
                    using (Image<Rgba32>  image = Image.Load<Rgba32>(pict.PictureData))
                    {
                        return File(MutateImageToStream(image, width, height), "image/jpeg");
                    }
                }
            }

            return File(MutateImageToStream(GetNoPhoto(), width,height), "image/jpeg");
            
             
            
         
        }
        // GET: Pictures
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pictures.ToListAsync());
        }

        // GET: Pictures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pictures == null)
            {
                return NotFound();
            }

            return View(pictures);
        }

        // GET: Pictures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PictureData")] Pictures pictures)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pictures);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pictures);
        }

        // GET: Pictures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures.FindAsync(id);
            if (pictures == null)
            {
                return NotFound();
            }
            return View(pictures);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PictureData")] Pictures pictures)
        {
            if (id != pictures.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pictures);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PicturesExists(pictures.Id))
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
            return View(pictures);
        }

        // GET: Pictures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pictures = await _context.Pictures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pictures == null)
            {
                return NotFound();
            }

            return View(pictures);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pictures = await _context.Pictures.FindAsync(id);
            _context.Pictures.Remove(pictures);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PicturesExists(int id)
        {
            return _context.Pictures.Any(e => e.Id == id);
        }
    }
}
