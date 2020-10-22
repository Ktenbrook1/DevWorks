using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [ForeignKey("Developer")]
        public int DevloperId { get; set; }
        public Developer Developer { get; set; }

        [ForeignKey("Listing")]
        public int ListingId { get; set; }
        public Listing Listing { get; set; }
    }
}
