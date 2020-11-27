using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }
        [Display(Name ="Message")]
        public string MessageToSend { get; set; }     
        public string EmployerName { get; set; } 
        public string DeveloperName { get; set; }
        public string EmployerEmail { get; set; }
        public string DeveloperEmail { get; set; }
        public string Sender { get; set; }
        [NotMapped]
        public string developerID { get; set; }
        [NotMapped]
        public string employerID { get; set; }

        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }

        [ForeignKey("Developer")]
        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
