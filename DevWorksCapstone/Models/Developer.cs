using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Developer
    {
        [Key]
        public int DeveloperId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Github")]
        public string GitHubLink { get; set; }
        [Display(Name = "Profile Picture")]
        public string ProfileImgURL { get; set; }
        public string Bio { get; set; }
        [Display(Name = "Rate I charge per hr")]
        public double RatePerHr { get; set; }
        [Display(Name = "Currently Under Contract")]
        public bool IsInContract { get; set; }
        public int AvgRating { get; set; }

        [NotMapped]
        public IList<string>? SelectedAbilities { get; set; }
        [NotMapped]
        public IList<SelectListItem> AllAbilities { get; set; }
        [NotMapped]
        public IList<Message> Messages { get; set; }
        [NotMapped]
        public IEnumerable<Listing> Listings { get; set; }
        [NotMapped]
        public SelectList ListingsForEmp { get; set; }
        [NotMapped]
        public int MyLisitng { get; set; }
        public ICollection<DeveloperAbilities> DevAbilities { get; set; }

        public Developer()
        {
            SelectedAbilities = new List<string>();
            AllAbilities = new List<SelectListItem>();
        }

        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }

        //[ForeignKey("Team")]
        //public int TeamId { get; set; }
        //public Team Team { get; set; }

        public ICollection<TeamOfDevs> Teams { get; set; }
    }
}
