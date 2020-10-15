using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class Ability
    {
        [Key]
        public int AbilityId { get; set; }
        public string AbilityName { get; set; }
        //public bool HasIt { get; set; }

        public ICollection<DeveloperAbilities> DeveloperAbilities { get; set; }
        public ICollection<EmployersWantedAbilities> EmployersWantedAbilities { get; set; }
    }
}
