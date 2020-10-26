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
using System.Net.Mail;
using System.Net;
using DevWorksCapstone.Models.MyAPIKeys;
using System.Security.Cryptography.X509Certificates;

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
                    if (developers.Contains(develope)) { }
                    else
                    {
                        developers.Add(develope);
                    }
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
            message.EmployerEmail = loggedInEmployer.Email;

            var DeveloperToContact = _context.Developers.Where(d => d.DeveloperId == id).SingleOrDefault();
            message.DeveloperId = DeveloperToContact.DeveloperId;
            message.DeveloperName = DeveloperToContact.UserName;
            message.DeveloperEmail = DeveloperToContact.Email;

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
            var teamHaveListing = _context.Teams.Where(t => t.ListingId == listing.ListingId).ToList();
            //   var developerToContract = _context.Developers.Where(d => d.DeveloperId == developer.DeveloperId).SingleOrDefault();
            var foundDev = _context.Developers.Where(d => d.DeveloperId == developer.DeveloperId).SingleOrDefault();
            TeamOfDevs ofDevs = new TeamOfDevs();
         //   var findDevsOnTeam = _context.TeamOfDevs.Where(t => t.TeamId = teamHaveListing[0].ListingId);

            if (teamHaveListing.Count != 0)
            {
                ofDevs.DeveloperId = foundDev.DeveloperId;
                ofDevs.TeamId = teamHaveListing[0].TeamId;
                _context.TeamOfDevs.Add(ofDevs);

                foundDev.IsInContract = true;
                _context.Update(foundDev);
                await _context.SaveChangesAsync();
            }
            else
            {
                Team team = new Team();
                team.ListingId = listing.ListingId;
                team.TeamIsAlive = true;
                team.TeamName = listing.Title;
                foundDev.IsInContract = true;
                _context.Update(foundDev);
                _context.Teams.Add(team);

                await _context.SaveChangesAsync();

                ofDevs.DeveloperId = foundDev.DeveloperId;
                ofDevs.TeamId = team.TeamId;
                _context.TeamOfDevs.Add(ofDevs);
          
                await _context.SaveChangesAsync();
            }
            //  SendEmail(developer);

            return RedirectToAction(nameof(Index));
        }
        public void SendEmail(Developer developer)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var fromEmail = loggedInEmployer.Email;
            var toEmail = developer.Email;
            var fromAddress = new MailAddress(fromEmail, loggedInEmployer.UserName);
            var toAddress = new MailAddress(toEmail, developer.UserName);
            string fromPassword = MyKeys.passcode;
            const string subject = "Hired On DevWorks";
            string body = "Congratulations " + loggedInEmployer.UserName + " Has Hired you for contract at " + loggedInEmployer.CompanyName + "!";

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message2 = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message2);
            }
        }

        public async Task<IActionResult> Team(int? id)
        {
            var findListing = _context.Listings.Where(l => l.ListingId == id).SingleOrDefault();
            var Team = _context.Teams.Where(t => t.ListingId == findListing.ListingId).SingleOrDefault();
            List<Developer> findDev = new List<Developer>();
            try
            {
                var DevsOnTeam = _context.TeamOfDevs.Where(tod => tod.TeamId == Team.TeamId).ToList();

                foreach (var devOnTeam in DevsOnTeam)
                {
                    var aDev = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId).SingleOrDefault();

                    findDev.Add(aDev);
                }
            }
            catch
            {

            }
            return View(findDev);
        }
        public async Task<IActionResult> Teams()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var findAllTeams = _context.Teams.Where(t => t.Listing.EmployerId == loggedInEmployer.EmployerId).ToList();
            if (findAllTeams == null)
            {
                IEnumerable<Team> team = new List<Team>();
                return View(team);
            }
            return View(findAllTeams);
        }
        public async Task<IActionResult> PleaseRateDevs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.Where(t => t.TeamId == id).SingleAsync();
            if (team == null)
            {
                return NotFound();
            }

            //var findListing = _context.Listings.Where(l => l.ListingId == id).SingleOrDefault();
            //var Team = _context.Teams.Where(t => t.ListingId == findListing.ListingId).SingleOrDefault();
            List<Developer> findDev = new List<Developer>();
            try
            {
                var DevsOnTeam = _context.TeamOfDevs.Where(tod => tod.TeamId == team.TeamId).ToList();

                foreach (var devOnTeam in DevsOnTeam)
                {
                    var aDev = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId).SingleOrDefault();
                    
                    findDev.Add(aDev);
                }
            }
            catch
            {

            }
            return View(findDev);
        }
        public async Task<IActionResult> MakeReview(int? id)
        {
            var findDev = _context.Developers.Where(d => d.DeveloperId == id).SingleOrDefault();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            var teamOfCurrentDev = _context.TeamOfDevs.Where(tod => tod.DeveloperId == findDev.DeveloperId).SingleOrDefault();
           
            Review review = new Review();
            review.DevloperId = findDev.DeveloperId;
            review.TeamId = teamOfCurrentDev.TeamId;
            review.WhoImRating = findDev.UserName;
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
            return RedirectToAction(nameof(Teams));
        }
        public async Task<IActionResult> MyReviews()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Developers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var findReviews = _context.Reviews.Where(r => r.WhoImRating == loggedInEmployer.UserName).ToList();
            return View(findReviews);
        }

        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var developerObj = _context.Developers.Where(e => e.DeveloperId == id).SingleOrDefault();
            var findReviews = _context.Reviews.Where(r => r.WhoImRating == developerObj.UserName).ToList();

            return View(findReviews);
        }
    }
}
