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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace DevWorksCapstone.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public EmployersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
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

                //Save image to wwwroot/image
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(employer.ProfileImgURL.FileName);
                string extension = Path.GetExtension(employer.ProfileImgURL.FileName);
                employer.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await employer.ProfileImgURL.CopyToAsync(fileStream);
                }

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
            var query = developers.OrderByDescending(developers => developers.AvgRating);
            
            return View(query);
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
            message.Sender = loggedInEmployer.IdentityUserId;

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

            return RedirectToAction("ViewConversation", new { id = message.developerID });
        }
        public async Task<IActionResult> Messages()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

            var myMessage = _context.Message.Where(m => m.EmployerId == loggedInEmployer.EmployerId).ToList();
            List<Developer> developers = new List<Developer>();
            foreach(var message in myMessage)
            {
                var developerMessaged = _context.Developers.Where(d => d.DeveloperId == message.DeveloperId).SingleOrDefault();
                if (developers.Contains(developerMessaged))
                {
                    continue;
                }
                else
                {
                    developers.Add(developerMessaged);
                }
            }

            return View(developers);
        }
        public async Task<IActionResult> ViewConversation(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var findDev = _context.Developers.Where(d => d.DeveloperId == id).SingleOrDefault();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
            var findconverstaion = _context.Message.Where(m => m.EmployerId == loggedInEmployer.EmployerId).ToList();

            List<Message> convoWithDev = new List<Message>();
            foreach(var message in findconverstaion)
            {
                if(message.DeveloperId == findDev.DeveloperId)
                {
                    convoWithDev.Add(message);
                }
                if (message.Sender == loggedInEmployer.IdentityUserId)
                {
                    message.employerID = message.Sender;
                }
                else
                {
                    message.developerID = message.Sender;
                }
            }
            return View(convoWithDev);
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

            string arrayOfStringAbilities = " ";
            int stringCount = 1;
            foreach (string ability in DeveloperToContract[0].SelectedAbilities)
            {         
                if(DeveloperToContract[0].SelectedAbilities.Count == stringCount)
                {
                    arrayOfStringAbilities = arrayOfStringAbilities + (ability );
                }
                else
                {
                    arrayOfStringAbilities = arrayOfStringAbilities + (ability + ", ");
                }               
                stringCount++;
            }
            var theDeveloper = DeveloperToContract[0];
            theDeveloper.arrayOfStringAbilities = arrayOfStringAbilities;
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
            var developerToContract = _context.Developers.Where(d => d.DeveloperId == developer.DeveloperId).SingleOrDefault();
            var foundDev = _context.Developers.Where(d => d.DeveloperId == developer.DeveloperId).SingleOrDefault();

            if (teamHaveListing.Count != 0)
            {
                var devs = Developers(teamHaveListing[0]);
                teamHaveListing[0].DevelopersOnTeam.Add(developerToContract);
                foundDev.TeamId = teamHaveListing[0].TeamId;
                foundDev.IsInContract = true;
                _context.Update(foundDev);
                await _context.SaveChangesAsync();
            }
            else
            {
                Team team = new Team()
                {
                    DevelopersOnTeam = new List<Developer>()
                };
                team.ListingId = listing.ListingId;
                team.TeamIsAlive = true;
                team.TeamName = listing.Title;
                

                team.DevelopersOnTeam.Add(developerToContract);
                //foundDev.TeamId = team.TeamId;
                foundDev.IsInContract = true;
                _context.Update(foundDev);
                _context.Teams.Add(team);

                await _context.SaveChangesAsync();
            }
            try
            {
                SendEmail(developer);
            }
            catch
            {

            }        

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

            var devs = Developers(Team);
            try
            {
                if(devs.Count > 0)
                {
                    foreach (Developer devOnTeam in Team.DevelopersOnTeam)
                    {
                        var developerOnTeam = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId);
                        findDev.Add(devOnTeam);
                    }
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

            var findListing = _context.Listings.Where(l => l.ListingId == team.ListingId).SingleOrDefault();
            var Team = _context.Teams.Where(t => t.ListingId == findListing.ListingId).SingleOrDefault();
            List<Developer> findDev = new List<Developer>();

            var devs = Developers(Team);
            try
            {
                foreach (Developer devOnTeam in Team.DevelopersOnTeam)
                {
                    var developerOnTeam = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId);
                    findDev.Add(devOnTeam);
                }
            }
            catch
            {

            }
            if(findDev.Count == 0)
            {
                Team.TeamIsAlive = false;
                _context.Teams.Update(Team);
                await _context.SaveChangesAsync();
            }
            return View(findDev);
        }
        public async Task<IActionResult> MakeReview(int? id)
        {
            var findDev = _context.Developers.Where(d => d.DeveloperId == id).SingleOrDefault();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();
           
            Review review = new Review();
            review.WhoImRating = findDev.UserName;
            review.DeveloperId = findDev.DeveloperId;
            review.EmployerId = loggedInEmployer.EmployerId;
            review.TeamCurrentlyOn = findDev.TeamId;
            return View(review);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReview(Review review)
        {
            var devToUpdate = _context.Developers.Where(d => d.DeveloperId == review.DeveloperId).SingleOrDefault();
            devToUpdate.IsInContract = false;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var allReviewsForDev = _context.Reviews.Where(r => r.WhoImRating == devToUpdate.UserName).ToList();
            int totalRating = 0;
            int count = 0;
            foreach (Review review1 in allReviewsForDev)
            {
                totalRating += review1.Rating;
                count++;
            }
            devToUpdate.AvgRating = totalRating / count;
            //var team = _context.Teams.Where(t => t.TeamId == review.TeamCurrentlyOn).SingleOrDefault();
            _context.Update(devToUpdate);
            await _context.SaveChangesAsync();

            var Team = _context.Teams.Where(t => t.TeamId == review.TeamCurrentlyOn).SingleOrDefault();
            List<Developer> findDev = new List<Developer>();

            var devs = Developers(Team);
            try
            {
                foreach (Developer devOnTeam in Team.DevelopersOnTeam)
                {
                    var developerOnTeam = _context.Developers.Where(d => d.DeveloperId == devOnTeam.DeveloperId);
                    findDev.Add(devOnTeam);
                }
            }
            catch { }

            Team.TeamIsAlive = false;
            _context.Teams.Update(Team);
            await _context.SaveChangesAsync();

            if (findDev.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("PleaseRateDevs", new { id = review.TeamCurrentlyOn });
            }
        }
        public async Task<IActionResult> MyReviews()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInEmployer = _context.Employers.Where(e => e.IdentityUserId == userId).SingleOrDefault();

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

        public List<Developer> Developers(Team team)
        {
            var findDevsOnTeam = _context.Developers.Where(d => d.TeamId == team.TeamId).ToList();
            List<Developer> foundDevs = new List<Developer>();
            foreach(Developer dev in findDevsOnTeam)
            {
                if(dev.IsInContract == true)
                {
                    foundDevs.Add(dev);
                }
            }
          
            return foundDevs;
        }
    }
}
