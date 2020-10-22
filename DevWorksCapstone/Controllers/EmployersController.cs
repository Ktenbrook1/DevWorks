using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWorksCapstone.Data;
using DevWorksCapstone.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DevWorksCapstone.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employers
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).Include(e => e.IdentityUser);
            if (loggedInEmployer.Count() == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                ViewData["EmployerExists"] = loggedInEmployer.Count() == 1;
                return View(loggedInEmployer);
            }
        }

        // GET: Employers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(m => m.EmployerId == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // GET: Employers/Create
        public IActionResult Create()
        {
            Employer employer = new Employer();
            return View(employer);
        }

        // POST: Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employer employer)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                employer.IdentityUserId = userId;

                _context.Employers.Add(employer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(employer);
        }

        // GET: Employers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers.FindAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employer employer)
        {
            if (id != employer.EmployerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerExists(employer.EmployerId))
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
            return View(employer);
        }

        // GET: Employers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(m => m.EmployerId == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employer = await _context.Employers.FindAsync(id);
            _context.Employers.Remove(employer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(int id)
        {
            return _context.Employers.Any(e => e.EmployerId == id);
        }

        public async Task<IActionResult> EmployerListings()
        {
            List<Listing> myListings = new List<Listing>();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            myListings = await _context.Listings.Where(l => l.EmployerId == loggedInEmployer.EmployerId).ToListAsync();

            return View(myListings);
        }
        public IActionResult CreateListing()
        {
            Listing listing = new Listing();
            listing.AllAbilities = GetAbilities();
            return View(listing);
        }
        public IList<SelectListItem> GetAbilities()
        {
            var allAbilities = _context.Abilities.ToList();
            List<SelectListItem> abilitiesAsSelectListItems = new List<SelectListItem>();

            foreach (Ability ability in allAbilities)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateListing(Listing listing)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var employerFound = _context.Employers.Where(e => e.IdentityUserId == userId).FirstOrDefault();
                listing.EmployerId = employerFound.EmployerId;
                listing.EmployerName = employerFound.UserName;
              
               

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();

                foreach (string ability in listing.SelectedAbilities)
                {
                    var selectedAbilities = _context.Abilities.Where(a => a.AbilityName == ability).SingleOrDefault();

                    EmployersWantedAbilities employersWantedAbilities = new EmployersWantedAbilities();
                    employersWantedAbilities.ListingId = listing.ListingId;
                    employersWantedAbilities.AbilityId = selectedAbilities.AbilityId;
                    _context.EmployersWantedAbilities.Add(employersWantedAbilities);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(listing);
        }
        public async Task<IActionResult> RecommendedDevelopers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }
            //Get developers with the highest rating
            //Get developers with the skills the are looking for based on their listing
     
            //  var findDevelopers = _context.DeveloperAbilities.Where(da => da.AbilityId == listing.EmployersWantedAbilities.)

            var currentListing = _context.Listings.Where(l => l.ListingId == listing.ListingId).ToList();

            currentListing[0].SelectedAbilities = _context.EmployersWantedAbilities
                .Where(ewa => ewa.ListingId == currentListing[0].ListingId)
                .Select(ewa => ewa.Ability.AbilityName)
                .ToList();

            var arrayStringAbilities = currentListing[0].SelectedAbilities;
            List<DeveloperAbilities> junctionOfAbilities = new List<DeveloperAbilities>();
            foreach (var ability in arrayStringAbilities)
            {
                var abilities = _context.DeveloperAbilities.Where(da => da.Ability.AbilityName == ability).ToList();
               foreach(var abilitypart2 in abilities)
                {
                    junctionOfAbilities.Add(abilitypart2);
                }
            }
            List<Developer> developers = new List<Developer>();
            foreach ( var item in junctionOfAbilities)
            {
                var developersWhoMatch = _context.Developers.Where(d => d.DeveloperId == item.DeveloperId).ToList();
                foreach(var develope in developersWhoMatch)
                {
                    developers.Add(develope);
                }
            }
           //WILL NEED TO INCORPORATE RATINGS SOON
            
            return View(developers);
        }

        public async Task<IActionResult> Contact(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Message message = new Message();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            message.EmployerId= loggedInEmployer.EmployerId;
            message.EmployerName = loggedInEmployer.UserName;

            var DeveloperToContact = _context.Developers.Where(d => d.DeveloperId == id).SingleOrDefault();
            message.DeveloperId = DeveloperToContact.DeveloperId;
            message.DeveloperName = DeveloperToContact.UserName;

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
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var myMessage = _context.Message.Where(m => m.EmployerId == loggedInEmployer.EmployerId).ToList();

            return View(myMessage);
        }

        public async Task<IActionResult> Hire(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var DeveloperToContract = _context.Developers.Where(d => d.DeveloperId == id).ToList();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            DeveloperToContract[0].SelectedAbilities = _context.DeveloperAbilities
              .Where(da => da.DeveloperId == DeveloperToContract[0].DeveloperId)
              .Select(da => da.Ability.AbilityName)
              .ToList();

            var arrayOfStringAbilities = DeveloperToContract[0].SelectedAbilities;
            var theDeveloper = DeveloperToContract[0];

            var allListing = _context.Listings.Where(l => l.EmployerId == loggedInEmployer.EmployerId);
            theDeveloper.Listings = allListing;

            var listings = theDeveloper.Listings;
            theDeveloper.ListingsForEmp = new SelectList(listings, "ListingId", "Description");
            return View(theDeveloper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hire(Developer developer)
        {
            var listing = _context.Listings.Where(l => l.ListingId == developer.MyLisitng).SingleOrDefault();
            var teamHaveListing = _context.Teams.Where(t => t.ListingId == listing.ListingId).SingleOrDefault();
            var developerToContract = _context.Developers.Where(d => d.DeveloperId == developer.DeveloperId).SingleOrDefault();

            if (teamHaveListing != null)
            {
                //add developer to existing Team
                teamHaveListing.ListingId = listing.ListingId;
                teamHaveListing.DevloperId = developerToContract.DeveloperId;
                _context.Teams.Add(teamHaveListing);
            }
            else
            {
                Team team = new Team();
                team.ListingId = listing.ListingId;
                team.DevloperId = developerToContract.DeveloperId;
                _context.Teams.Add(team);
            }
            return RedirectToAction(nameof(Team));
        }

        public async Task<IActionResult> Team(int? id)
        {
            return View();
        }
    }
}
