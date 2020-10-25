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
        public string TeamName { get; set; }
        public bool TeamIsAlive { get; set; }


        [ForeignKey("Listing")]
        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        public ICollection<TeamOfDevs> DevelopersOnTeam { get; set; }

        public ICollection<Developer> DevelopersOnTeam2 { get; set; }
    }
}
