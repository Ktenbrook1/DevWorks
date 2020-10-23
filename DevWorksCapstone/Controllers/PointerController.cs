using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevWorksCapstone.Data;
using DevWorksCapstone.Models;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace DevWorksCapstone.Controllers
{
    public class PointerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PointerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Employer employerLoggedIn = _context.Employers.Where(e => e.IdentityUserId == userId).FirstOrDefault();
            var developerLoggedIn = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();

            if(employerLoggedIn != null)
            {
                return RedirectToAction("Messages", "Employers");
            }
            else
            {
                return RedirectToAction("Messages", "Developers");
            }
         
        }
        public IActionResult HomePage()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Employer employerLoggedIn = _context.Employers.Where(e => e.IdentityUserId == userId).FirstOrDefault();
            var developerLoggedIn = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();

            if (employerLoggedIn != null)
            {
                return RedirectToAction("Index", "Employers");
            }
            else
            {
                return RedirectToAction("HomePage", "Developers");
            }

        }

        public IActionResult TeamPage()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Employer employerLoggedIn = _context.Employers.Where(e => e.IdentityUserId == userId).FirstOrDefault();
            var developerLoggedIn = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();

            if (employerLoggedIn != null)
            {
                return RedirectToAction("Teams", "Employers");
            }
            else
            {
                return RedirectToAction("Team", "Developers");
            }
        }

        public IActionResult Profile()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Employer employerLoggedIn = _context.Employers.Where(e => e.IdentityUserId == userId).FirstOrDefault();
            var developerLoggedIn = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();

            if (employerLoggedIn != null)
            {
                return RedirectToAction("Index", "Employers");
            }
            else
            {
                return RedirectToAction("Index", "Developers");
            }
        }
    }
}
