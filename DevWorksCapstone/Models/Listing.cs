﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Listing
    {
        [Key]
        public int ListingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Rate Wanted")]
        public double RateLookingFor { get; set; }
        [Display(Name = "Openings")]
        public int PositionsOpen { get; set; }

        [NotMapped]
        public IList<string>? SelectedAbilities { get; set; }
        [NotMapped]
        public IList<SelectListItem> AllAbilities { get; set; }
        public Listing()
        {
            SelectedAbilities = new List<string>();
            AllAbilities = new List<SelectListItem>();
        }

        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }
        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; }

        public DateTime DateStarting
        {
            get
            {
                return this.StartDate.HasValue
                   ? this.StartDate.Value
                   : DateTime.Now;
            }

            set { this.StartDate = value; }
        }
        public DateTime DateEnding
        {
            get
            {
                return this.EndDate.HasValue
                   ? this.EndDate.Value
                   : DateTime.Now;
            }

            set { this.EndDate = value; }
        }

        public ICollection<EmployersWantedAbilities> EmployersWantedAbilities { get; set; }
    }
}
