using DIExample.Infrastructure;
using DIExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DIExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentRepo _repo;

        public HomeController(ILogger<HomeController> logger, IStudentRepo Repo)
        {
            _logger = logger;
            _repo = Repo;
        }

        public IActionResult Index()
        {
            var items = _repo.GetAll();
            return View(items);
        }
        public IActionResult Details(int id)
        {
            var items = _repo.GetByID(id);
            return View(items);
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
