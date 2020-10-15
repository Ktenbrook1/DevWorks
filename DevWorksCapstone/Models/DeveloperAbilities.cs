using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class DeveloperAbilities
    {
        [Key, Column(Order = 1)]
        public int AbilityId { get; set; }
        public Ability Ability { get; set; }


        [Key, Column(Order = 2)]
        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
