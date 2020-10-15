using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class EmployersWantedAbilities
    {
        [Key, Column(Order = 1)]
        public int AbilityId { get; set; }
        public Ability Ability { get; set; }


        [Key, Column(Order = 2)]
        public int ListingId { get; set; }
        public Listing Listing { get; set; }
    }
}
