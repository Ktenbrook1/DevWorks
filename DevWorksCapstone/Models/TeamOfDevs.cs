using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevWorksCapstone.Models
{
    public class TeamOfDevs
    {
        [Key, Column(Order = 1)]
        public int TeamId { get; set; }
        public Team Team{ get; set; }


        [Key, Column(Order = 2)]
        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
