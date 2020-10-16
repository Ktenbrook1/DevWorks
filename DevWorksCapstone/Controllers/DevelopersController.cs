using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWorksCapstone.Data;
using DevWorksCapstone.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DevWorksCapstone.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DevelopersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevelopersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Developers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Developers.Include(d => d.IdentityUser);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            if (loggedInDeveloper == null)
            {
                return RedirectToAction("Create");

            }

            var loggedInDeveloper2 = _context.Developers.Where(c => c.IdentityUserId == userId).Include(c => c.IdentityUser);

            ViewData["DeveloperExists"] = loggedInDeveloper2.Count() == 1;

            return View(loggedInDeveloper2);
        }

        // GET: Developers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .Include(d => d.IdentityUser)
                .FirstOrDefaultAsync(m => m.DeveloperId == id);
            if (developer == null)
            {
                return NotFound();
            }

            return View(developer);
        }

        // GET: Developers/Create
        public IActionResult Create()
        {
            Developer developer = new Developer();
            {
                developer.AllAbilities = GetAbilities();
            }
            return View(developer);
        }

        public IList<SelectListItem> GetAbilities()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "FrontEnd", Value = "FrontEnd" },
                new SelectListItem { Text = "BackEnd", Value = "BackEnd" },
                new SelectListItem { Text = "App Developer", Value = "App Developer" },
                new SelectListItem { Text = "React", Value = "React" }
            };
        }

        // POST: Developers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Developer developer)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                developer.IdentityUserId = userId;
                
                _context.Developers.Add(developer);
                await _context.SaveChangesAsync();

                var selectedDeveloper = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();
                foreach (string ability in developer.SelectedAbilities)
                {
                    var selectedAbilities = _context.Abilities.Where(a => a.AbilityName == ability).SingleOrDefault();

                    DeveloperAbilities developerAbilities = new DeveloperAbilities();
                    developerAbilities.DeveloperId = selectedDeveloper.DeveloperId;
                    developerAbilities.AbilityId = selectedAbilities.AbilityId;
                    _context.DeveloperAbilities.Add(developerAbilities);
                    _context.SaveChanges();
                }
                //return RedirectToAction(nameof(Index));
            }
            return View(developer);
        }

        // GET: Developers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers.FindAsync(id);
            if (developer == null)
            {
                return NotFound();
            }
           
            return View(developer);
        }

        // POST: Developers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Developer developer)
        {
            if (id != developer.DeveloperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeveloperExists(developer.DeveloperId))
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
            return View(developer);
        }

        // GET: Developers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .Include(d => d.IdentityUser)
                .FirstOrDefaultAsync(m => m.DeveloperId == id);
            if (developer == null)
            {
                return NotFound();
            }

            return View(developer);
        }

        // POST: Developers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var developer = await _context.Developers.FindAsync(id);
            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeveloperExists(int id)
        {
            return _context.Developers.Any(e => e.DeveloperId == id);
        }
    }
}
