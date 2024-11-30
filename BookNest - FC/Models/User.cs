using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BookNest.Models
{
    public class User : IdentityUser
    {
        public bool IsRestricted { get; set; } = false; // ব্যবহারকারীর নিষিদ্ধ অবস্থা

        public string FullName { get; set; } // Add this line

        public ICollection<BookIssue> BookIssues { get; set; }
    }
}
