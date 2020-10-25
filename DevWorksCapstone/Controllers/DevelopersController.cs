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
                ViewData["DeveloperExists"] = loggedInDeveloper.Count() == 1;
              //  return RedirectToAction(nameof(HomePage));
                return View(loggedInDeveloper);
            }
        }

        public async Task<IActionResult> HomePage()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Developer> loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).Include(e => e.IdentityUser).ToList();

            if (loggedInDeveloper.Count() == 0)
            {
                return RedirectToAction("Create");
            }

            loggedInDeveloper[0].SelectedAbilities = _context.DeveloperAbilities
              .Where(da => da.DeveloperId == loggedInDeveloper[0].DeveloperId)
              .Select(da => da.Ability.AbilityName)
              .ToList();

            var arrayOfStringAbilities = loggedInDeveloper[0].SelectedAbilities;

            var allListings = _context.Listings.AsNoTracking();

            List<Listing> PerfectMatch = new List<Listing>();
            List<Listing> PartialMatch = new List<Listing>();
            List<Listing> PotentialMatch = new List<Listing>();
            foreach (var listing in allListings)
            {
                List<Listing> currentListing = _context.Listings.Where(l => l.ListingId == listing.ListingId).ToList();

                currentListing[0].SelectedAbilities = _context.EmployersWantedAbilities
                    .Where(ewa => ewa.ListingId == currentListing[0].ListingId)
                    .Select(ewa => ewa.Ability.AbilityName)
                    .ToList();

                var stringCurrentListingAbilities = currentListing[0].SelectedAbilities;
                int points = 0;

                foreach (var abilityForListing in stringCurrentListingAbilities)
                {
                    var hasAbility = arrayOfStringAbilities.Contains(abilityForListing);
                    if(hasAbility == true)
                    {
                        points++;
                    }
                }

                if (points == stringCurrentListingAbilities.Count)
                {
                    foreach (var list in currentListing)
                    {
                        if (PerfectMatch.Contains(list)) { }
                        else
                        {
                            PerfectMatch.Add(list);
                        }
                    }
                }
                else if (points > 0)
                {
                    foreach (var list in currentListing)
                    {
                        if (PartialMatch.Contains(list)) { }
                        else
                        {
                            PartialMatch.Add(list);
                        }
                    }
                }
                else
                {
                    foreach (var list in currentListing)
                    {
                        if (PotentialMatch.Contains(list)) { }
                        else
                        {
                            PotentialMatch.Add(list);
                        }
                    }
                }
            }
            List<Listing> FinalOrganizedList = new List<Listing>();
            foreach(var list in PerfectMatch)
            {
                FinalOrganizedList.Add(list);
            }
            foreach(var list in PartialMatch)
            {
                FinalOrganizedList.Add(list);
            }
            foreach(var list in PotentialMatch)
            {
                FinalOrganizedList.Add(list);
            }

            return View(FinalOrganizedList);
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
                return RedirectToAction(nameof(HomePage));
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

        public async Task<IActionResult> Contact(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Message message = new Message();


            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInDeveloper = _context.Developers.Where(d => d.IdentityUserId == userId).SingleOrDefault();
            message.DeveloperId = loggedInDeveloper.DeveloperId;
            message.DeveloperName = loggedInDeveloper.UserName;


            var EmployerToContact = _context.Employers.Where(e => e.EmployerId == id).SingleOrDefault();
            message.EmployerId = EmployerToContact.EmployerId;
            message.EmployerName = EmployerToContact.UserName;

            return View(message);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Message message)
        {
            _context.Message.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Messages));
        }
        public async Task<IActionResult> Messages()
        {
            
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var myMessage = _context.Message.Where(m => m.DeveloperId == loggedInDeveloper.DeveloperId).ToList();

            //var listing = _context.Listings.Where(l => l.ListingId == message.ListingId).ToList();

            return View(myMessage);
        }
        //Takes in a listing Id so I will have to revise this
        public async Task<IActionResult> Team()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            List<Developer> findDev = new List<Developer>();
            try
            {
                var findTeam = _context.TeamOfDevs.Where(tod => tod.DeveloperId == loggedInDeveloper.DeveloperId).SingleOrDefault();
                var team = _context.Teams.Where(t => t.TeamId == findTeam.TeamId).SingleOrDefault();
                var DevsOnTeam = _context.TeamOfDevs.Where(tod => tod.TeamId == team.TeamId).ToList();

                foreach (var devOnTeam in DevsOnTeam)
                {
                    var aDev = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId).SingleOrDefault();

                    findDev.Add(aDev);
                }
            }
            catch {
            
            }

            return View(findDev);
        }
        public async Task<IActionResult> MakeReview()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInDeveloper = _context.Developers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            var findTeam = _context.TeamOfDevs.Where(tod => tod.DeveloperId == loggedInDeveloper.DeveloperId).SingleOrDefault();
            var team = _context.Teams.Where(t => t.TeamId == findTeam.TeamId).SingleOrDefault();
            var listing = _context.Listings.Where(l => l.ListingId == team.ListingId).SingleOrDefault();
            var employer = _context.Employers.Where(e => e.EmployerId == listing.EmployerId).SingleOrDefault();

            Review review = new Review();
            review.DevloperId = loggedInDeveloper.DeveloperId;
            review.TeamId = team.TeamId;
            review.WhoImRating = employer.UserName;
            return View(review);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReview(Review review)
        {
            var devJustReviewed = _context.TeamOfDevs.Where(d => d.DeveloperId == review.DevloperId).SingleOrDefault();
            var devToUpdate = _context.Developers.Where(d => d.DeveloperId == devJustReviewed.DeveloperId).SingleOrDefault();
            devToUpdate.IsInContract = false;
            _context.Update(devToUpdate);
            _context.TeamOfDevs.Remove(devJustReviewed);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomePage));
        }
    }
}
