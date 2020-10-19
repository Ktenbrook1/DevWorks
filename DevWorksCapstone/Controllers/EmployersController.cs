﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevWorksCapstone.Data;
using DevWorksCapstone.Models;
using System.Security.Claims;

namespace DevWorksCapstone.Controllers
{
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

        //[HttpPost, ActionName("RecommendedDevelopers")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RecommendedDevelopers(Listing listing)
        //{
        //    //Get developers with the highest rating
        //    //Get developers with the skills the are looking for based on their listing
        //    List<Developer> developers = new List<Developer>();
        //    //  var findDevelopers = _context.DeveloperAbilities.Where(da => da.AbilityId == listing.EmployersWantedAbilities.)

        //    var currentListing = _context.Listings.Where(l => l.ListingId == listing.ListingId).ToList();

        //    currentListing[0].SelectedAbilities = _context.EmployersWantedAbilities
        //        .Where(ewa => ewa.ListingId == currentListing[0].ListingId)
        //        .Select(ewa => ewa.Ability.AbilityName)
        //        .ToList();

        //    var arrayStringAbilities = currentListing[0].SelectedAbilities;

        //    foreach(var ability in arrayStringAbilities)
        //    {
        //        var developersWithWantedAbilities = _context.DeveloperAbilities.Where(da => da.Ability.AbilityName == ability);
        //    }       
        //  //  var arrangeDevelopers = _context.Developers.
        //    return View(developers);
        //}
    }
}
