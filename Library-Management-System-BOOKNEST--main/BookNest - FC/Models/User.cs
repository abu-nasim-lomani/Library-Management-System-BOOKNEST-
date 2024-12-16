using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BookNest.Models
{
    public class User : IdentityUser
    {
        public bool IsRestricted { get; set; } = false; 

        public string FullName { get; set; } 

        public ICollection<BookIssue> BookIssues { get; set; }
    }
}
