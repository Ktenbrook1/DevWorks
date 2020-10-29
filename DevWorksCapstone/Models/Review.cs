using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        [Display(Name = "Who I'm Rating")]
        public string WhoImRating { get; set; }
        public int Rating { get; set; }
        [Display(Name = "Review")]
        public string ReviewGiven { get; set; }

        [ForeignKey("Developer")]
        public int DevloperId { get; set; }
        public Developer Developer { get; set; }

        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }
    }
}
