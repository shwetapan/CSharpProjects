using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRegExample.Data;
using UserRegExample.Models;
using UserRegExample.ViewModels;

namespace UserRegExample.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public TestController(ILogger<TestController> logger, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            ManageUserRoles UserRoles = new ManageUserRoles();
            var user = _context.Users.Where(x => x.Id == Id).SingleOrDefault();
            var userInRole = _context.UserRoles.Where(x => x.UserId == Id).Select(x => x.RoleId).ToList();
            var userInClaims = _context.UserClaims.Where(x => x.UserId == Id).Select(x => x.ClaimValue).ToList();


            UserRoles.roles = await _roleManager.Roles.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id,
                Selected = userInRole.Contains(x.Id)
            }).ToListAsync();           
            UserRoles.AppUser = user;

            UserRoles.ApplicationClaims = ClaimStore.All.Select(x => new SelectListItem()
            {
                Text = x.Type,
                Value = x.Value,
                Selected = userInRole.Contains(x.Value)
            }).ToList();
            return View(UserRoles);
        }

        [HttpPost]
        public IActionResult Edit(ManageUserRoles model)
        {
            var selectedRoleId = model.roles.Where(x => x.Selected).Select(x => x.Value);
            var AlreadyExistRoleId = _context.UserRoles.Where(x => x.UserId == model.AppUser.Id).Select(x => x.RoleId).ToList();
            var toAdd = selectedRoleId.Except(AlreadyExistRoleId);
            var toRemove = AlreadyExistRoleId.Except(selectedRoleId);

            foreach (var item in toRemove)
            {
                _context.UserRoles.Remove(new IdentityUserRole<string>
                {
                    RoleId = item,
                    UserId = model.AppUser.Id
                });
            }
            foreach (var item in toAdd)
            {
                _context.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = item,
                    UserId = model.AppUser.Id
                });
            }

            // for claims
            var selectedClaimValue = model.ApplicationClaims.Where(x => x.Selected).Select(x => x.Value);
            var AlreadyExistClaims = _context.UserClaims.Where(x => x.UserId == model.AppUser.Id).Select(x => x.Id.ToString()).ToList();
            var toAddClaims = selectedClaimValue.Except(AlreadyExistClaims);
            var toRemoveClaims = AlreadyExistClaims.Except(selectedClaimValue);

            foreach (var item in toRemoveClaims)
            {
                _context.UserClaims.Remove(new IdentityUserClaim<string>
                {
                    Id = Convert.ToInt32(item),
                    UserId = model.AppUser.Id
                });
            }
            foreach (var item in toAddClaims)
            {
                _context.UserClaims.Add(new IdentityUserClaim<string>
                {
                    UserId = model.AppUser.Id,
                    ClaimValue=item,
                    ClaimType=item
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

     }
}
