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
        public string MessageToSend { get; set; }

        [ForeignKey("Listing")]
        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        [ForeignKey("Developer")]
        public int DevloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
