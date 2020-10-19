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
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Developer> loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).Include(e => e.IdentityUser).ToList();  

            if (loggedInDeveloper.Count() == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                loggedInDeveloper[0].SelectedAbilities = _context.DeveloperAbilities
               .Where(da => da.DeveloperId == loggedInDeveloper[0].DeveloperId)
               .Select(da => da.Ability.AbilityName)
               .ToList();

                ViewData["DeveloperExists"] = loggedInDeveloper.Count() == 1;
                return View(loggedInDeveloper);
            }
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
            developer.AllAbilities = GetAbilities();
            return View(developer);
        }

        public IList<SelectListItem> GetAbilities()
        {
            var allAbilities = _context.Abilities.ToList();
            List<SelectListItem> abilitiesAsSelectListItems = new List<SelectListItem>();

            foreach(Ability ability in allAbilities)
            {
                SelectListItem abilityItem = new SelectListItem()
                {
                    Text = ability.AbilityName,
                    Value = ability.AbilityName
                };

                abilitiesAsSelectListItems.Add(abilityItem);
            }
            return abilitiesAsSelectListItems;
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
                foreach(string ability in selectedDeveloper.SelectedAbilities)
                {
                    var selectedAbilities = _context.Abilities.Where(a => a.AbilityName == ability).SingleOrDefault();

                    DeveloperAbilities developerAbilities = new DeveloperAbilities();
                    developerAbilities.DeveloperId = selectedDeveloper.DeveloperId;
                    developerAbilities.AbilityId = selectedAbilities.AbilityId;
                    _context.DeveloperAbilities.Add(developerAbilities);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
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
            developer.AllAbilities = GetAbilities();
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

                    var selectedDeveloper = _context.Developers.Where(d => d.IdentityUserId == developer.IdentityUserId).SingleOrDefault();
                    foreach (string ability in selectedDeveloper.SelectedAbilities)
                    {
                        var abilitiesThatMatch = _context.DeveloperAbilities.Where(da => da.Ability.AbilityName == ability).ToList();
                        var developerThatMatchAbilities = abilitiesThatMatch.Where(am => am.DeveloperId == developer.DeveloperId).ToList();
                      
                        if(developerThatMatchAbilities != null) { }
                        else
                        {
                            var selectedAbilities = _context.Abilities.Where(a => a.AbilityName == ability).SingleOrDefault();

                            DeveloperAbilities developerAbilities = new DeveloperAbilities();
                            developerAbilities.DeveloperId = selectedDeveloper.DeveloperId;
                            developerAbilities.AbilityId = selectedAbilities.AbilityId;
                            _context.DeveloperAbilities.Add(developerAbilities);
                            _context.SaveChanges();
                        }                      
                    }
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
