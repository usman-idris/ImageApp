using ImageApp.Data;
using ImageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var images = _db.Images.ToList();
            return View(images);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult AddProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfile(Images images)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    byte[] pic = null;
                    using (var filestream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            filestream.CopyTo(memoryStream);
                            pic = memoryStream.ToArray();
                        }
                    }
                    images.Picture = pic;
                }
                _db.Images.Add(images);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(images);
        }
    }
}
