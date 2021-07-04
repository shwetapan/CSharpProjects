using DemoSearchApp.Data;
using DemoSearchApp.Helper;
using DemoSearchApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DemoSearchApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _contect;


        public ProductController(ILogger<ProductController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _contect = context;
        }

        public async Task<IActionResult> Index(int pageNumber=1,string searchProductName = null)
        {
            var searchResult = _contect.Products.AsNoTracking();
            if (!string.IsNullOrEmpty(searchProductName))
            {


                searchResult = _contect.Products.Where(x => x.Title.Contains(searchProductName)).AsNoTracking();
                if (searchResult.Count() == 0)
                {
                    ViewBag.products = "Search Product Not Found";
                    return View();
                }
            }
            int pageSize = 3;
            return View(await PaginatedList<Product>.CreateAsync(searchResult,pageNumber,pageSize)); 
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
    }
}
