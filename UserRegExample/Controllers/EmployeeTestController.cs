using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegExample.Repo;

namespace UserRegExample.Controllers
{
    public class EmployeeTestController : Controller
    {
        private readonly IEmployee _repository;

        public EmployeeTestController(IEmployee repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var employees = _repository.GetAll();
            return View(employees);
        }
    }
}
