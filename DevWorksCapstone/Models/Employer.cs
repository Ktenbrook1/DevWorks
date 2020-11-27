using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Employer
    {
        [Key]
        public int EmployerId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        [StringLength(250)]
        public string ImageName { get; set; }
        [NotMapped]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImgURL { get; set; }

        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
