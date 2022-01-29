using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UserAPI.Models
{
    public class Claim
    {
        public Guid ClaimId { get; set; }
        public string ClaimDescription { get; set; }
    }
}
